using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    [SerializeField] private List<KitchenObjectSO> validIngredients;
    private readonly HashSet<KitchenObjectSO> ingredients = new();

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;

        public OnIngredientAddedEventArgs(KitchenObjectSO kitchenObjectSO)
        {
            this.kitchenObjectSO = kitchenObjectSO;
        }
    }

    public bool TryAddIngredient(KitchenObjectSO ingredient)
    {
        if (!validIngredients.Contains(ingredient) || ingredients.Contains(ingredient))
        {
            return false;
        }
        AddIngredientServerRpc(validIngredients.IndexOf(ingredient));
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int validIngredientsIndex)
    {
        AddIngredientClientRpc(validIngredientsIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int validIngredientsIndex)
    {
        KitchenObjectSO ingredient = validIngredients[validIngredientsIndex];
        ingredients.Add(ingredient);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs(ingredient));
    }

    public HashSet<KitchenObjectSO> GetIngredients()
    {
        return ingredients;
    }
}
