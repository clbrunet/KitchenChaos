using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    [SerializeField] private List<KitchenObjectSO> validIngredients;
    private HashSet<KitchenObjectSO> ingredients = new HashSet<KitchenObjectSO>();

    public bool TryAddIngredient(KitchenObjectSO ingredient)
    {
        if (!validIngredients.Contains(ingredient))
        {
            return false;
        }
        return ingredients.Add(ingredient);
    }
}
