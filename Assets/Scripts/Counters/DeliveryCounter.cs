using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject() is PlateKitchenObject)
            {
                DeliveryManager.Instance.Deliver(player.GetKitchenObject() as PlateKitchenObject);
                player.GetKitchenObject().DestroySelf();
            }
        }
    }
}
