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
        if (!player.HasKitchenObject())
        {
            if (HasKitchenObject())
            {
                kitchenObject.SetParent(player);
            }
            else
            {
                Instantiate(kitchenObjectSO.prefab).SetParent(player);
                OnObjectInstantiation?.Invoke(this, EventArgs.Empty);
            }
        }
        else if (!HasKitchenObject())
        {
            player.GetKitchenObject().SetParent(this);
        }
    }
}
