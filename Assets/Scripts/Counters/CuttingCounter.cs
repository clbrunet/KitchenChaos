using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgression
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOs;

    private int cutsCount;
    public event EventHandler OnCut;

    public event EventHandler<IHasProgression.OnProgressionEventArgs> OnProgression;

    public override void Interact(Player player)
    {
        if (cutsCount != 0)
        {
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

    public override void InteractAlternate(Player player)
    {
        if (!HasKitchenObject() || player.HasKitchenObject())
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
            if (input == cuttingRecipeSO.input)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

    private void Cut(CuttingRecipeSO recipe)
    {
        OnCut?.Invoke(this, EventArgs.Empty);
        cutsCount++;
        if (cutsCount == recipe.cutsNeeded)
        {
            kitchenObject.DestroySelf();
            Instantiate(recipe.output.prefab).SetParent(this);
            cutsCount = 0;
        }
        OnProgression?.Invoke(this, new IHasProgression.OnProgressionEventArgs((float)cutsCount / (float)recipe.cutsNeeded));
    }
}
