using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (parent.HasKitchenObject())
        {
            return;
        }
        this.parent?.ClearKitchenObject();
        this.parent = parent;
        parent.SetKitchenObject(this);
        transform.SetParent(parent.GetKitchenObjectParent(), false);
    }
}
