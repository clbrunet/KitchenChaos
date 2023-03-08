using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plate;
    [SerializeField] private PlateIconUI iconTemplate;

    private void Start()
    {
        plate.OnIngredientAdded += Plate_OnIngredientAdded;
    }

    private void Plate_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        PlateIconUI icon = Instantiate(iconTemplate, transform);
        icon.SetSprite(e.kitchenObjectSO);
        icon.gameObject.SetActive(true);
    }
}
