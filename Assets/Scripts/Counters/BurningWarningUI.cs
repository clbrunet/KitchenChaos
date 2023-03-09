using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningWarningUI : MonoBehaviour
{
    private StoveCounter stoveCounter;

    private void Awake()
    {
        stoveCounter = GetComponentInParent<StoveCounter>();
        stoveCounter.OnBurningStarted += StoveCounter_OnBurningStarted;
        stoveCounter.OnTurningOff += StoveCounter_OnTurningOff;
        gameObject.SetActive(false);
    }

    private void StoveCounter_OnBurningStarted(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void StoveCounter_OnTurningOff(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }
}
