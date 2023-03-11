using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    [SerializeField] protected Transform topPoint;
    protected KitchenObject kitchenObject;

    public static event EventHandler OnObjectDrop;

    public abstract void Interact(Player player);

    public virtual void InteractAlternate(Player player)
    {
    }

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
        if (kitchenObject != null)
        {
            OnObjectDrop?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public bool TryGrabKitchenObject(Player player)
    {
        if (!HasKitchenObject())
        {
            return false;
        }
        if (!player.HasKitchenObject())
        {
            kitchenObject.SetParent(player);
            return true;
        }
        KitchenObject playerKitchenObject = player.GetKitchenObject();
        if (playerKitchenObject is PlateKitchenObject)
        {
            PlateKitchenObject playerPlate = playerKitchenObject as PlateKitchenObject;
            if (playerPlate.TryAddIngredient(kitchenObject.GetKitchenObjectSO()))
            {
                kitchenObject.DestroySelf();
                return true;
            }
        }
        return false;
    }

    public bool TryPutKitchenObject(Player player)
    {
        if (!player.HasKitchenObject())
        {
            return false;
        }
        if (!HasKitchenObject())
        {
            player.GetKitchenObject().SetParent(this);
            return true;
        }
        if (kitchenObject is PlateKitchenObject)
        {
            PlateKitchenObject counterPlate = kitchenObject as PlateKitchenObject;
            if (counterPlate.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                player.GetKitchenObject().DestroySelf();
                return true;
            }
        }
        return false;
    }
}
