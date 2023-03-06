using System;
using System.Collections;
using System.Collections.Generic;
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
        if (!validIngredients.Contains(ingredient))
        {
            return false;
        }
        if (!ingredients.Add(ingredient))
        {
            return false;
        }
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs(ingredient));
        return true;
    }
}
