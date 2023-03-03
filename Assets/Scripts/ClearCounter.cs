using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private KitchenObject kitchenObject;

    public void Interact(Player player)
    {
        if (kitchenObject == null)
        {
            Instantiate(kitchenObjectSO.prefab).SetParent(this);
        }
        else
        {
            kitchenObject.SetParent(player);
        }
    }

    public Transform GetKitchenObjectParent()
    {
        return topPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
