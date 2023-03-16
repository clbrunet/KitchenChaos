using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    public event EventHandler OnTryingToJoin;
    public event EventHandler OnFailedToJoin;

    public const int MAX_PLAYER_COUNT = 4;
    private NetworkList<PlayerData> playerDatas;
    public event EventHandler OnPlayerDatasChanged;

    public const string PLAYER_PREFS_PLAYER_NAME = "PlayerName";
    private string playerName;

    [SerializeField] private Color[] playerSelectableColorArray;

    public event EventHandler OnUnityAuthenticationInitialized;

    public static bool isSingleplayer = false;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of MultiplayerManager");
        Instance = this;
        playerDatas = new NetworkList<PlayerData>();
        playerDatas.OnListChanged += PlayerDatas_OnListChanged;
        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME, "PlayerName" + UnityEngine.Random.Range(100, 1000));

        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        yield return null;
        OnUnityAuthenticationInitialized += MultiplayerManager_OnUnityAuthenticationInitialized;
        InitalizeUnityAuthentication();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_HostConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_HostOnClientConnectedCallback;
        }
    }

    private async void InitalizeUnityAuthentication()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            OnUnityAuthenticationInitialized?.Invoke(this, EventArgs.Empty);
            return;
        }
        try
        {
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (RequestFailedException e)
        {
            Debug.Log(e);
        }
        OnUnityAuthenticationInitialized?.Invoke(this, EventArgs.Empty);
    }

    private void MultiplayerManager_OnUnityAuthenticationInitialized(object sender, EventArgs e)
    {
        if (isSingleplayer)
        {
            StartHost();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    private void PlayerDatas_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDatasChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_HostConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_HostOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_HostOnClientDisconnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_HostConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
        NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        connectionApprovalResponse.Approved = false;
        if (SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString()
            && SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectionScene.ToString())
        {
            connectionApprovalResponse.Reason = "The game has already started";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_COUNT)
        {
            connectionApprovalResponse.Reason = "The game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }

    private void NetworkManager_HostOnClientConnectedCallback(ulong clientId)
    {
        playerDatas.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
            name = playerName,
            id = AuthenticationService.Instance.PlayerId,
        });
    }

    private void NetworkManager_HostOnClientDisconnectedCallback(ulong clientId)
    {
        for (int i = 0; i < playerDatas.Count; i++)
        {
            if (playerDatas[i].clientId == clientId)
            {
                LobbyManager.Instance.KickPlayer(playerDatas[i].id.ToString());
                playerDatas.RemoveAt(i);
            }
        }
    }

    public void StartClient()
    {
        OnTryingToJoin?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_ClientOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_ClientOnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_ClientOnClientConnectedCallback(ulong obj)
    {
        SetPlayerNameServerRpc(playerName);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_ClientOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_ClientOnClientDisconnectCallback;
    }

    private void NetworkManager_ClientOnClientDisconnectCallback(ulong obj)
    {
        OnFailedToJoin?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_ClientOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_ClientOnClientDisconnectCallback;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int index = GetIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData senderPlayerData = playerDatas[index];
        senderPlayerData.name = playerName;
        playerDatas[index] = senderPlayerData;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME, playerName);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int index = GetIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData senderPlayerData = playerDatas[index];
        senderPlayerData.id = playerId;
        playerDatas[index] = senderPlayerData;
    }

    public bool IsIndexConnected(int index)
    {
        return index < playerDatas.Count;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDatas)
        {
            if (clientId == playerData.clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerDataFromIndex(int index)
    {
        return playerDatas[index];
    }

    public int GetIndexFromClientId(ulong clientId)
    {
        int i = 0;
        foreach (PlayerData playerData in playerDatas)
        {
            if (clientId == playerData.clientId)
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    public Color GetPlayerSelectableColor(int colorId)
    {
        return playerSelectableColorArray[colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            return;
        }
        int index = GetIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData senderPlayerData = playerDatas[index];
        senderPlayerData.colorId = colorId;
        playerDatas[index] = senderPlayerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerData playerData in playerDatas)
        {
            if (colorId == playerData.colorId)
            {
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerSelectableColorArray.Length; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_HostOnClientDisconnectedCallback(clientId);
    }
}
