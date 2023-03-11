using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public event EventHandler OnObjectInstantiation;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject() && !HasKitchenObject())
        {
            KitchenObject.Spawn(kitchenObjectSO, player);
            ObjectInstantiationServerRpc();
            return;
        }
        if (TryGrabKitchenObject(player))
        {
            return;
        }
        if (TryPutKitchenObject(player))
        {
            return;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ObjectInstantiationServerRpc()
    {
        ObjectInstantiationClientRpc();
    }

    [ClientRpc]
    private void ObjectInstantiationClientRpc()
    {
        OnObjectInstantiation?.Invoke(this, EventArgs.Empty);
    }
}
