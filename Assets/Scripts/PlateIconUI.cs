using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] private Image ingredientImage;

    public void SetSprite(KitchenObjectSO kitchenObjectSO)
    {
        ingredientImage.sprite = kitchenObjectSO.sprite;
    }
}
