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

    public event EventHandler OnBurningStarted;

    public override void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            StoveRecipeSO recipe = GetRecipe(kitchenObject.GetKitchenObjectSO());
            if (recipe != null)
            {
                return;
            }
            if (TryGrabKitchenObject(player))
            {
                StopCoroutine(cookCoroutine);
                OnProgression?.Invoke(this, new IHasProgression.OnProgressionEventArgs(0f));
                OnTurningOff?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
        if (player.HasKitchenObject())
        {
            StoveRecipeSO recipe = GetRecipe(player.GetKitchenObject().GetKitchenObjectSO());
            if (recipe == null)
            {
                return;
            }
            if (TryPutKitchenObject(player))
            {
                cookCoroutine = StartCoroutine(Cook(recipe));
                return;
            }
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
            if (!GameManager.Instance.IsPlaying())
            {
                OnTurningOff?.Invoke(this, EventArgs.Empty);
                yield break;
            }
            elapsed += Time.deltaTime;
            OnProgression?.Invoke(this, new IHasProgression.OnProgressionEventArgs(elapsed / recipe.cookTime));
        }
        kitchenObject.DestroySelf();
        KitchenObject.Spawn(recipe.cooked, this);
        OnBurningStarted?.Invoke(this, EventArgs.Empty);
        elapsed = 0f;
        while (elapsed < recipe.cookTime)
        {
            yield return null;
            if (!GameManager.Instance.IsPlaying())
            {
                OnTurningOff?.Invoke(this, EventArgs.Empty);
                yield break;
            }
            elapsed += Time.deltaTime;
            OnProgression?.Invoke(this, new IHasProgression.OnProgressionEventArgs(elapsed / recipe.cookTime));
        }
        kitchenObject.DestroySelf();
        KitchenObject.Spawn(recipe.burned, this);
        OnTurningOff?.Invoke(this, EventArgs.Empty);
    }
}
