using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(Player player)
    {
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
