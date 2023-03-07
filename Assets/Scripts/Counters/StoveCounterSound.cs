using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    private StoveCounter stoveCounter;
    private AudioSource audioSource;

    private void Awake()
    {
        stoveCounter = GetComponentInParent<StoveCounter>();
        audioSource = GetComponent<AudioSource>();
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
        audioSource.Play();
    }

    private void StoveCounter_OnTurningOff(object sender, System.EventArgs e)
    {
        audioSource.Pause();
    }
}
