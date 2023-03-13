using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class ReadyClientsManager : NetworkBehaviour
{
    public static ReadyClientsManager Instance { get; private set; }

    private readonly Dictionary<ulong, bool> serverReadyClientsDictionary = new();

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
        serverReadyClientsDictionary[serverRpcParams.Receive.SenderClientId] = isClientReady;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!serverReadyClientsDictionary.ContainsKey(clientId)
                || !serverReadyClientsDictionary[clientId])
            {
                return;
            }
        }
        Loader.LoadNetwork(Loader.Scene.GameScene);
    }
}
