using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;
    [SerializeField] private Image bar;

    private void Start()
    {
        cuttingCounter.OnCut += CuttingCounter_OnCut;
        gameObject.SetActive(false);
    }

    private void CuttingCounter_OnCut(object sender, CuttingCounter.OnCutEventArgs e)
    {
        bar.fillAmount = e.cutsProgress;
        gameObject.SetActive(0 < bar.fillAmount && bar.fillAmount < 1);
    }
}
