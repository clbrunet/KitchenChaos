using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public void Interact()
    {
        KitchenObject kitchenObject = Instantiate(kitchenObjectSO.prefab, topPoint);
        Debug.Log("Interaction with " + name + ", " + kitchenObject.GetKitchenObjectSO().objectName + " spawned");
    }
}
