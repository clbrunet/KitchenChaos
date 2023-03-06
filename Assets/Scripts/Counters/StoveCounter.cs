using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgression
{
    [SerializeField] private StoveRecipeSO[] stoveRecipeSOs;

    private Coroutine cookCoroutine;

    public event EventHandler<IHasProgression.OnProgressionEventArgs> OnProgression;

    public event EventHandler OnTurningOn;
    public event EventHandler OnTurningOff;

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject() && !HasKitchenObject())
        {
            StoveRecipeSO recipe = GetRecipe(player.GetKitchenObject().GetKitchenObjectSO());
            if (recipe == null)
            {
                return;
            }
            player.GetKitchenObject().SetParent(this);
            cookCoroutine = StartCoroutine(Cook(recipe));
        }
        else if (!player.HasKitchenObject() && HasKitchenObject())
        {
            StoveRecipeSO recipe = GetRecipe(kitchenObject.GetKitchenObjectSO());
            if (recipe != null)
            {
                return;
            }
            StopCoroutine(cookCoroutine);
            OnProgression?.Invoke(this, new IHasProgression.OnProgressionEventArgs(0f));
            OnTurningOff?.Invoke(this, EventArgs.Empty);
            kitchenObject.SetParent(player);
        }
    }

    private StoveRecipeSO GetRecipe(KitchenObjectSO uncooked)
    {
        foreach (StoveRecipeSO stoveRecipeSO in stoveRecipeSOs)
        {
            if (uncooked == stoveRecipeSO.uncooked)
            {
                return stoveRecipeSO;
            }
        }
        return null;
    }

    private IEnumerator Cook(StoveRecipeSO recipe)
    {
        OnTurningOn?.Invoke(this, EventArgs.Empty);
        float elapsed = 0f;
        while (elapsed < recipe.cookTime)
        {
            yield return null;
            elapsed += Time.deltaTime;
            OnProgression?.Invoke(this, new IHasProgression.OnProgressionEventArgs(elapsed / recipe.cookTime));
        }
        kitchenObject.DestroySelf();
        Instantiate(recipe.cooked.prefab).SetParent(this);
        elapsed = 0f;
        while (elapsed < recipe.cookTime)
        {
            yield return null;
            elapsed += Time.deltaTime;
            OnProgression?.Invoke(this, new IHasProgression.OnProgressionEventArgs(elapsed / recipe.cookTime));
        }
        kitchenObject.DestroySelf();
        Instantiate(recipe.burned.prefab).SetParent(this);
        OnTurningOff?.Invoke(this, EventArgs.Empty);
    }
}
