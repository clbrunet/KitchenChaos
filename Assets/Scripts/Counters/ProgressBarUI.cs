using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] GameObject hasProgressionGameObject;
    private IHasProgression hasProgression;
    [SerializeField] private Image bar;

    private void Awake()
    {
        hasProgression = hasProgressionGameObject.GetComponent<IHasProgression>();
        Assert.IsNotNull(hasProgression);
    }

    private void Start()
    {
        hasProgression.OnProgression += HasProgression_OnProgression;
        gameObject.SetActive(false);
    }

    private void HasProgression_OnProgression(object sender, IHasProgression.OnProgressionEventArgs e)
    {
        bar.fillAmount = e.progression;
        gameObject.SetActive(0 < bar.fillAmount && bar.fillAmount < 1);
    }
}
