using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] protected Transform topPoint;
    protected KitchenObject kitchenObject;

    public abstract void Interact(Player player);

    public Transform GetKitchenObjectParent()
    {
        return topPoint;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
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
