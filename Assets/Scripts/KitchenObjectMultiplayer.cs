using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class KitchenObjectMultiplayer : NetworkBehaviour
{
    public static KitchenObjectMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of KitchenObjectMultiplayer");
        Instance = this;
    }

    public void Spawn(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        int kitchenObjectSOIndex = kitchenObjectListSO.kitchenObjectSOs.IndexOf(kitchenObjectSO);
        SpawnServerRpc(kitchenObjectSOIndex, kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        if (kitchenObjectParent.HasKitchenObject())
        {
            return;
        }
        KitchenObjectSO kitchenObjectSO = kitchenObjectListSO.kitchenObjectSOs[kitchenObjectSOIndex];
        KitchenObject kitchenObject = Instantiate(kitchenObjectSO.prefab);
        kitchenObject.NetworkObject.Spawn();
        kitchenObject.SetParent(kitchenObjectParent);
    }

    public void Destroy(KitchenObject kitchenObject)
    {
        DestroyServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        if (kitchenObjectNetworkObject == null)
        {
            return;
        }
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        DestroyClientRpc(kitchenObject.GetParent().GetNetworkObject());
        kitchenObject.NetworkObject.Despawn();
    }

    [ClientRpc]
    public void DestroyClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObjectParent.ClearKitchenObject();
    }
}
