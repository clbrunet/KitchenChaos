using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    private const string OPEN_CLOSE = "OpenClose";

    private Animator animator;
    private ContainerCounter containerCounter;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        containerCounter = GetComponentInParent<ContainerCounter>();
    }

    private void OnEnable()
    {
        containerCounter.OnObjectInstantiation += ContainerCounter_OnObjectInstantiation;
    }

    private void OnDisable()
    {
        containerCounter.OnObjectInstantiation -= ContainerCounter_OnObjectInstantiation;
    }

    private void ContainerCounter_OnObjectInstantiation(object sender, System.EventArgs e)
    {
        animator.SetTrigger(OPEN_CLOSE);
    }
}
