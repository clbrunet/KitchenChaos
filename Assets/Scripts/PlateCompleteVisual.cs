using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plate;

    [Serializable]
    private struct KitchenObjectSOGameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }
    [SerializeField] private KitchenObjectSOGameObject[] kitchenObjectSOGameObjectArray;

    private void Start()
    {
        foreach (KitchenObjectSOGameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectArray)
        {
            kitchenObjectSOGameObject.gameObject.SetActive(false);
        }
        plate.OnIngredientAdded += Plate_OnIngredientAdded;
    }

    private void Plate_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (KitchenObjectSOGameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectArray)
        {
            if (e.kitchenObjectSO == kitchenObjectSOGameObject.kitchenObjectSO)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(true);
                return;
            }
        }
    }
}
