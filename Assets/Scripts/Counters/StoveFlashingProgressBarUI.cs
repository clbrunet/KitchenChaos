using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveFlashingProgressBarUI : MonoBehaviour
{
    private StoveCounter stoveCounter;
    private Animator animator;
    private const string IS_FLASHING = "IsFlashing";
    private bool isBurningStarted = false;

    private void Awake()
    {
        stoveCounter = GetComponentInParent<StoveCounter>();
        animator = GetComponent<Animator>();
        stoveCounter.OnBurningStarted += StoveCounter_OnBurningStarted;
        stoveCounter.OnTurningOff += StoveCounter_OnTurningOff;
    }

    private void Update()
    {
        animator.SetBool(IS_FLASHING, isBurningStarted);
    }

    private void StoveCounter_OnBurningStarted(object sender, System.EventArgs e)
    {
        isBurningStarted = true;
    }

    private void StoveCounter_OnTurningOff(object sender, System.EventArgs e)
    {
        isBurningStarted = false;
    }
}
