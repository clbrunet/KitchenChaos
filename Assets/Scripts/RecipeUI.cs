using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeName;
    [SerializeField] private Transform iconsContainer;
    [SerializeField] private Image iconTemplate;

    private void Start()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipe(RecipeSO recipe)
    {
        recipeName.text = recipe.recipeName;
        foreach (Transform child in iconsContainer)
        {
            if (child == iconTemplate.transform)
            {
                continue;
            }
            Destroy(child.gameObject);
        }
        foreach (KitchenObjectSO ingredient in recipe.ingredients)
        {
            Image icon = Instantiate(iconTemplate, iconsContainer);
            icon.gameObject.SetActive(true);
            icon.sprite = ingredient.sprite;
        }
    }
}
