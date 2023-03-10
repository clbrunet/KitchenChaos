using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class DeliveryManager : NetworkBehaviour
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

    private int successfulDeliveryCount = 0;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of DeliveryManager");
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e)
    {
        if (!IsServer || e.state != GameManager.State.GamePlaying)
        {
            return;
        }
        const int STARTING_RECIPES_COUNT = 2;
        for (int i = 0; i < STARTING_RECIPES_COUNT; i++)
        {
            AddRandomWaitingRecipe();
        }
    }

    private void Update()
    {
        if (!IsServer || !GameManager.Instance.IsPlaying() || waitingRecipeSOs.Count == WAITING_RECIPES_MAX)
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
        AddWaitingRecipeClientRpc(UnityEngine.Random.Range(0, recipeListSO.recipeSOs.Count));
    }

    [ClientRpc]
    private void AddWaitingRecipeClientRpc(int recipeIndex)
    {
        RecipeSO recipe = recipeListSO.recipeSOs[recipeIndex];
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
                DeliverValidRecipeServerRpc(i);
                return true;
            }
            i++;
        }
        DeliverInvalidRecipeServerRpc();
        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverValidRecipeServerRpc(int waitingRecipeIndex)
    {
        DeliverValidRecipeClientRpc(waitingRecipeIndex);
    }

    [ClientRpc]
    private void DeliverValidRecipeClientRpc(int waitingRecipeIndex)
    {
        waitingRecipeSOs.RemoveAt(waitingRecipeIndex);
        OnWaitingRecipeDelivered?.Invoke(this, new OnWaitingRecipeDeliveredEventArgs(waitingRecipeIndex));
        OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
        successfulDeliveryCount++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverInvalidRecipeServerRpc()
    {
        DeliverInvalidRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverInvalidRecipeClientRpc()
    {
        OnDeliveryFail?.Invoke(this, EventArgs.Empty);
    }

    public int GetSuccessfulDeliveryCount()
    {
        return successfulDeliveryCount;
    }
}
