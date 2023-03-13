using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    public event EventHandler OnTryingToJoin;
    public event EventHandler OnFailedToJoin;

    private const int MAX_PLAYER_COUNT = 4;
    private NetworkList<PlayerData> playerDatas;
    public event EventHandler OnPlayerDatasChanged;

    [SerializeField] private Color[] playerSelectableColorArray;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of MultiplayerManager");
        Instance = this;
        playerDatas = new NetworkList<PlayerData>();
        playerDatas.OnListChanged += PlayerDatas_OnListChanged;

        DontDestroyOnLoad(gameObject);
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

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_HostConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_HostOnClientConnectedCallback;
        }
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
        playerDatas.Add(new PlayerData {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
        });
    }

    private void NetworkManager_HostOnClientDisconnectedCallback(ulong clientId)
    {
        for (int i = 0; i < playerDatas.Count; i++)
        {
            if (playerDatas[i].clientId == clientId)
            {
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
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_ClientOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_ClientOnClientDisconnectCallback;
    }

    private void NetworkManager_ClientOnClientDisconnectCallback(ulong obj)
    {
        OnFailedToJoin?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_ClientOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_ClientOnClientDisconnectCallback;
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
}
