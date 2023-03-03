using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOs;

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject() && !HasKitchenObject())
        {
            player.GetKitchenObject().SetParent(this);
        }
        else if (!player.HasKitchenObject() && HasKitchenObject())
        {
            kitchenObject.SetParent(player);
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (!HasKitchenObject())
        {
            return;
        }
        KitchenObjectSO output = GetOutputFromInput(kitchenObject.GetKitchenObjectSO());
        if (output ==  null)
        {
            return;
        }
        kitchenObject.DestroySelf();
        Instantiate(output.prefab).SetParent(this);
    }

    private KitchenObjectSO GetOutputFromInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOs)
        {
            if (cuttingRecipeSO.input == kitchenObjectSO)
            {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }
}
