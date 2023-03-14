using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class ReadyClientsManager : NetworkBehaviour
{
    public static ReadyClientsManager Instance { get; private set; }

    private readonly Dictionary<ulong, bool> readyClientsDictionary = new();
    public event EventHandler OnReadyClientsChanged;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of ReadyClientsManager");
        Instance = this;
    }

    public void SetClientReady(bool isClientReady = true)
    {
        SetClientReadyServerRpc(isClientReady);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetClientReadyServerRpc(bool isClientReady, ServerRpcParams serverRpcParams = default)
    {
        SetClientReadyClientRpc(isClientReady, serverRpcParams.Receive.SenderClientId);
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!readyClientsDictionary.ContainsKey(clientId)
                || !readyClientsDictionary[clientId])
            {
                return;
            }
        }
        LobbyManager.Instance.DeleteLobby();
        Loader.LoadNetwork(Loader.Scene.GameScene);
    }

    [ClientRpc]
    private void SetClientReadyClientRpc(bool isClientReady, ulong clientId)
    {
        readyClientsDictionary[clientId] = isClientReady;
        OnReadyClientsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsClientReady(ulong clientId)
    {
        return readyClientsDictionary.ContainsKey(clientId) && readyClientsDictionary[clientId];
    }
}
