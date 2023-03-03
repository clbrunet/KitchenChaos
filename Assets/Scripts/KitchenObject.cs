using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent parent;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public void SetParent(IKitchenObjectParent parent)
    {
        Assert.IsFalse(parent.HasKitchenObject(), "parent already had a kitchen object");
        this.parent?.ClearKitchenObject();
        this.parent = parent;
        parent.SetKitchenObject(this);
        transform.SetParent(parent.GetKitchenObjectParent(), false);
    }

    public void DestroySelf()
    {
        parent.ClearKitchenObject();
        Destroy(gameObject);
    }
}
