using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed;

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObject.Destroy(player.GetKitchenObject());
            TrashServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TrashServerRpc()
    {
        TrashClientRpc();
    }

    [ClientRpc]
    private void TrashClientRpc()
    {
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }
}
