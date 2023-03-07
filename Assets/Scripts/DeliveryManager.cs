using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    private readonly List<RecipeSO> waitingRecipeSOs = new();
    private const int WAITING_RECIPES_MAX = 4;
    private float elapsed = 0f;
    private const float WAITING_RECIPE_SPAWN_DURATION = 4f;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of DeliveryManager");
        Instance = this;
    }

    private void Start()
    {
        const int STARTING_RECIPES_COUNT = 2;
        for (int i = 0; i < STARTING_RECIPES_COUNT; i++)
        {
            waitingRecipeSOs.Add(GetRandomRecipe());
        }
    }

    private void Update()
    {
        if (waitingRecipeSOs.Count == WAITING_RECIPES_MAX)
        {
            return;
        }
        elapsed += Time.deltaTime;
        if (elapsed > WAITING_RECIPE_SPAWN_DURATION)
        {
            elapsed = 0f;
            waitingRecipeSOs.Add(GetRandomRecipe());
        }
    }

    private RecipeSO GetRandomRecipe()
    {
        return recipeListSO.recipeSOs[Random.Range(0, recipeListSO.recipeSOs.Count)];
    }

    public bool Deliver(PlateKitchenObject plate)
    {
        int i = 0;
        foreach (RecipeSO recipe in waitingRecipeSOs)
        {
            HashSet<KitchenObjectSO> plateIngredients = plate.GetIngredients();
            if (recipe.ingredients.Count != plateIngredients.Count)
            {
                i++;
                continue;
            }
            bool isValid = true;
            foreach (KitchenObjectSO ingredient in recipe.ingredients)
            {
                if (!plateIngredients.Contains(ingredient))
                {
                    isValid = false;
                    break;
                }
            }
            if (isValid)
            {
                waitingRecipeSOs.RemoveAt(i);
                return true;
            }
            i++;
        }
        return false;
    }
}
