using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOs;

    private int cutsCount;
    public event EventHandler<OnCutEventArgs> OnCut;
    public class OnCutEventArgs : EventArgs
    {
        public float cutsProgress;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject() && !HasKitchenObject())
        {
            player.GetKitchenObject().SetParent(this);
        }
        else if (!player.HasKitchenObject() && HasKitchenObject())
        {
            CuttingRecipeSO recipe = GetRecipe(kitchenObject.GetKitchenObjectSO());
            if (recipe == null || cutsCount == 0)
            {
                kitchenObject.SetParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (!HasKitchenObject())
        {
            return;
        }
        CuttingRecipeSO recipe = GetRecipe(kitchenObject.GetKitchenObjectSO());
        if (recipe ==  null)
        {
            return;
        }
        Cut(recipe);
    }

    private CuttingRecipeSO GetRecipe(KitchenObjectSO input)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOs)
        {
            if (cuttingRecipeSO.input == input)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

    private void Cut(CuttingRecipeSO recipe)
    {
        cutsCount++;
        if (cutsCount == recipe.cutsNeeded)
        {
            kitchenObject.DestroySelf();
            Instantiate(recipe.output.prefab).SetParent(this);
            cutsCount = 0;
        }
        OnCut?.Invoke(this, new OnCutEventArgs
        {
            cutsProgress = (float)cutsCount / (float)recipe.cutsNeeded
        });
    }
}
