using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private FollowTransform followTransform;

    private IKitchenObjectParent parent;

    private void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public IKitchenObjectParent GetParent()
    {
        return parent;
    }

    public void SetParent(IKitchenObjectParent parent)
    {
        SetParentServerRpc(parent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetParentServerRpc(NetworkObjectReference parentNetworkObjectReference)
    {
        SetParentClientRpc(parentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetParentClientRpc(NetworkObjectReference parentNetworkObjectReference)
    {
        parentNetworkObjectReference.TryGet(out NetworkObject parentNetworkObject);
        IKitchenObjectParent parent = parentNetworkObject.GetComponent<IKitchenObjectParent>();
        Assert.IsFalse(parent.HasKitchenObject(), "parent already had a kitchen object");
        this.parent?.ClearKitchenObject();
        this.parent = parent;
        parent.SetKitchenObject(this);
        followTransform.SetTarget(parent.GetKitchenObjectParent());
    }

    //public void DestroySelf()
    //{
    //    //parent.ClearKitchenObject();
    //    //Destroy(gameObject);
    //    print("DestroySelf: " + name);
    //    DestroySelfServerRpc();
    //}

    //[ServerRpc(RequireOwnership = false)]
    //private void DestroySelfServerRpc()
    //{
    //    print("DestroySelfServerRpc: " + name);
    //    DestroySelfClientRpc();
    //}

    //[ClientRpc]
    //private void DestroySelfClientRpc()
    //{
    //    MonoBehaviour a = parent as MonoBehaviour;
    //    print("DestroySelfClientRpc parent: " + a.name);
    //}

    public static void Spawn(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        KitchenObjectMultiplayer.Instance.Spawn(kitchenObjectSO, kitchenObjectParent);
    }

    public static void Destroy(KitchenObject kitchenObject)
    {
        KitchenObjectMultiplayer.Instance.Destroy(kitchenObject);
    }
}
