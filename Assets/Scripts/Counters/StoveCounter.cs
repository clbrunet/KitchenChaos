using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgression
{
    [SerializeField] private StoveRecipeSO[] stoveRecipeSOs;

    private Coroutine serverCookCoroutine;

    public event EventHandler<IHasProgression.OnProgressionEventArgs> OnProgression;

    public event EventHandler OnTurningOn;
    public event EventHandler OnTurningOff;

    public event EventHandler OnBurningStarted;

    public NetworkVariable<float> cookingElapsedTime = new(0f);
    public float currentRecipeCookTime = 0f;

    public override void OnNetworkSpawn()
    {
        cookingElapsedTime.OnValueChanged += (float previousValue, float newValue) =>
        {
            OnProgression?.Invoke(this, new IHasProgression.OnProgressionEventArgs(cookingElapsedTime.Value / currentRecipeCookTime));
        };
    }

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
                StopCookingServerRpc();
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
                CookServerRpc();
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

    [ServerRpc(RequireOwnership = false)]
    private void CookServerRpc()
    {
        StoveRecipeSO recipe = GetRecipe(kitchenObject.GetKitchenObjectSO());
        SetCurrentRecipeCookTimeClientRpc();
        serverCookCoroutine = StartCoroutine(Cook(recipe));
    }

    [ClientRpc]
    private void SetCurrentRecipeCookTimeClientRpc()
    {
        StoveRecipeSO recipe = GetRecipe(kitchenObject.GetKitchenObjectSO());
        currentRecipeCookTime = recipe.cookTime;
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopCookingServerRpc()
    {
        StopCoroutine(serverCookCoroutine);
        cookingElapsedTime.Value = 0f;
        TurnOffServerRpc();
    }

    private IEnumerator Cook(StoveRecipeSO recipe)
    {
        TurnOnServerRpc();
        cookingElapsedTime.Value = 0f;
        while (cookingElapsedTime.Value < recipe.cookTime)
        {
            yield return null;
            if (!GameManager.Instance.IsPlaying())
            {
                TurnOffServerRpc();
                yield break;
            }
            cookingElapsedTime.Value += Time.deltaTime;
        }
        KitchenObject.Destroy(kitchenObject);
        KitchenObject.Spawn(recipe.cooked, this);
        StartBurningServerRpc();
        cookingElapsedTime.Value = 0f;
        while (cookingElapsedTime.Value < recipe.cookTime)
        {
            yield return null;
            if (!GameManager.Instance.IsPlaying())
            {
                TurnOffServerRpc();
                yield break;
            }
            cookingElapsedTime.Value += Time.deltaTime;
        }
        KitchenObject.Destroy(kitchenObject);
        KitchenObject.Spawn(recipe.burned, this);
        TurnOffServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TurnOnServerRpc()
    {
        TurnOnClientRpc();
    }

    [ClientRpc]
    private void TurnOnClientRpc()
    {
        OnTurningOn?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartBurningServerRpc()
    {
        StartBurningClientRpc();
    }

    [ClientRpc]
    private void StartBurningClientRpc()
    {
        OnBurningStarted?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TurnOffServerRpc()
    {
        TurnOffClientRpc();
    }

    [ClientRpc]
    private void TurnOffClientRpc()
    {
        OnTurningOff?.Invoke(this, EventArgs.Empty);
    }
}
