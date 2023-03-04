using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    private StoveCounter stoveCounter;
    [SerializeField] private GameObject[] stoveOnGameObjects;

    private void Awake()
    {
        stoveCounter = GetComponentInParent<StoveCounter>();
    }

    private void OnEnable()
    {
        stoveCounter.OnTurningOn += StoveCounter_OnTurningOn;
        stoveCounter.OnTurningOff += StoveCounter_OnTurningOff;
    }

    private void OnDisable()
    {
        stoveCounter.OnTurningOn -= StoveCounter_OnTurningOn;
        stoveCounter.OnTurningOff -= StoveCounter_OnTurningOff;
    }

    private void StoveCounter_OnTurningOn(object sender, System.EventArgs e)
    {
        foreach (GameObject stoveOnGameObject in stoveOnGameObjects)
        {
            stoveOnGameObject.SetActive(true);
        }
    }

    private void StoveCounter_OnTurningOff(object sender, System.EventArgs e)
    {
        foreach (GameObject stoveOnGameObject in stoveOnGameObjects)
        {
            stoveOnGameObject.SetActive(false);
        }
    }
}
