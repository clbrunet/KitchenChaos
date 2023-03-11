using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of DeliveryCounter");
        Instance = this;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject() is PlateKitchenObject)
            {
                DeliveryManager.Instance.Deliver(player.GetKitchenObject() as PlateKitchenObject);
                KitchenObject.Destroy(player.GetKitchenObject());
            }
        }
    }
}
