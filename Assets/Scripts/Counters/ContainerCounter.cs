using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public event EventHandler OnObjectInstantiation;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject() && !HasKitchenObject())
        {
            Instantiate(kitchenObjectSO.prefab).SetParent(player);
            OnObjectInstantiation?.Invoke(this, EventArgs.Empty);
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
}
