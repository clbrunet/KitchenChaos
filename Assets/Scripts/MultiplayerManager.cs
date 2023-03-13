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
        playerDatas.Add(new PlayerData { clientId = clientId });
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

    public PlayerData GetPlayerDataFromIndex(int index)
    {
        return playerDatas[index];
    }
}
