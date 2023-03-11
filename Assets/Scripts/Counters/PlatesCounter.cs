using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO plateSO;

    private float elapsed = 0f;
    private const float PLATE_SPAWN_DURATION = 5f;
    private int platesCount = 0;
    private const int MAX_PLATES_COUNT = 5;

    public event EventHandler OnPlateSpawn;
    public event EventHandler OnPlateGrab;

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying() || platesCount == MAX_PLATES_COUNT)
        {
            return;
        }
        elapsed += Time.deltaTime;
        if (elapsed > PLATE_SPAWN_DURATION)
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
            KitchenObject.Spawn(plateSO, player);
        }
    }
}