using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private GameObject plateVisualPrefab;

    private PlatesCounter platesCounter;
    [SerializeField] private Transform topPoint;

    private readonly Stack<GameObject> plates = new();

    private void Awake()
    {
        platesCounter = GetComponentInParent<PlatesCounter>();
    }

    private void OnEnable()
    {
        platesCounter.OnPlateSpawn += PlatesCounter_OnPlateSpawn;
        platesCounter.OnPlateGrab += PlatesCounter_OnPlateGrab;
    }

    private void OnDisable()
    {
        platesCounter.OnPlateSpawn -= PlatesCounter_OnPlateSpawn;
        platesCounter.OnPlateGrab -= PlatesCounter_OnPlateGrab;
    }

    private void PlatesCounter_OnPlateSpawn(object sender, System.EventArgs e)
    {
        GameObject plate = Instantiate(plateVisualPrefab, topPoint);
        plate.transform.localPosition = new Vector3(0f, 0.1f * plates.Count, 0f);
        plates.Push(plate);
    }

    private void PlatesCounter_OnPlateGrab(object sender, System.EventArgs e)
    {
        Destroy(plates.Pop());
    }
}
