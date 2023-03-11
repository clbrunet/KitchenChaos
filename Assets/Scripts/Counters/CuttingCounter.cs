using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgression
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOs;

    private int cutsCount;
    public event EventHandler OnCut;

    public event EventHandler<IHasProgression.OnProgressionEventArgs> OnProgression;

    public static event EventHandler OnAnyCut;

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
        TryCutServerRpc();
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

    [ServerRpc(RequireOwnership = false)]
    private void TryCutServerRpc()
    {
        CuttingRecipeSO recipe = GetRecipe(kitchenObject.GetKitchenObjectSO());
        if (recipe ==  null)
        {
            return;
        }
        CutClientRpc(recipe.cutsNeeded);
        if (cutsCount == 0)
        {
            KitchenObject.Destroy(kitchenObject);
            KitchenObject.Spawn(recipe.output, this);
        }
    }

    [ClientRpc]
    private void CutClientRpc(int recipeCutsNeeded)
    {
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        cutsCount++;
        if (cutsCount == recipeCutsNeeded)
        {
            cutsCount = 0;
        }
        OnProgression?.Invoke(this, new IHasProgression.OnProgressionEventArgs((float)cutsCount / (float)recipeCutsNeeded));
    }
}
