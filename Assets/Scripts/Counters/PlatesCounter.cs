using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO plateSO;

    private float elapsed = 0f;
    private const float plateSpawnDuration = 5f;
    private int platesCount = 0;
    [SerializeField] private int maxPlatesCount = 5;

    public event EventHandler OnPlateSpawn;
    public event EventHandler OnPlateGrab;

    private void Update()
    {
        if (platesCount == maxPlatesCount)
        {
            return;
        }
        elapsed += Time.deltaTime;
        if (elapsed > plateSpawnDuration)
        {
            elapsed = 0f;
            platesCount++;
            OnPlateSpawn?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject() && platesCount > 0)
        {
            platesCount--;
            OnPlateGrab?.Invoke(this, EventArgs.Empty);
            Instantiate(plateSO.prefab).SetParent(player);
        }
    }
}