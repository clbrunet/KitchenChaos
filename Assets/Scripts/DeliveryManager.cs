using System;
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

    public event EventHandler<OnWaitingRecipeAddedEventArgs> OnWaitingRecipeAdded;
    public class OnWaitingRecipeAddedEventArgs
    {
        public RecipeSO recipe;

        public OnWaitingRecipeAddedEventArgs(RecipeSO recipe)
        {
            this.recipe = recipe;
        }
    }
    public event EventHandler<OnWaitingRecipeDeliveredEventArgs> OnWaitingRecipeDelivered;
    public class OnWaitingRecipeDeliveredEventArgs
    {
        public int index;

        public OnWaitingRecipeDeliveredEventArgs(int index)
        {
            this.index = index;
        }
    }

    public event EventHandler OnDeliverySuccess;
    public event EventHandler OnDeliveryFail;

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
            AddRandomWaitingRecipe();
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
            AddRandomWaitingRecipe();
        }
    }

    private void AddRandomWaitingRecipe()
    {
        RecipeSO recipe = recipeListSO.recipeSOs[UnityEngine.Random.Range(0, recipeListSO.recipeSOs.Count)];
        waitingRecipeSOs.Add(recipe);
        OnWaitingRecipeAdded?.Invoke(this, new OnWaitingRecipeAddedEventArgs(recipe));
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
                OnWaitingRecipeDelivered?.Invoke(this, new OnWaitingRecipeDeliveredEventArgs(i));
                OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
                return true;
            }
            i++;
        }
        OnDeliveryFail?.Invoke(this, EventArgs.Empty);
        return false;
    }
}
