using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private RecipeUI template;

    private void Start()
    {
        template.gameObject.SetActive(false);
        DeliveryManager.Instance.OnWaitingRecipeAdded += DeliveryManager_OnWaitingRecipeAdded;
        DeliveryManager.Instance.OnWaitingRecipeDelivered += DeliveryManager_OnWaitingRecipeDelivered;
    }

    private void DeliveryManager_OnWaitingRecipeAdded(object sender, DeliveryManager.OnWaitingRecipeAddedEventArgs e)
    {
        RecipeUI recipeUI = Instantiate(template, container);
        recipeUI.SetRecipe(e.recipe);
        recipeUI.gameObject.SetActive(true);
    }

    private void DeliveryManager_OnWaitingRecipeDelivered(object sender, DeliveryManager.OnWaitingRecipeDeliveredEventArgs e)
    {
        Destroy(container.GetChild(e.index + 1).gameObject);
    }
}
