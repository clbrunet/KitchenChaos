using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
        if (target != null )
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }

    private void LateUpdate()
    {
        if (target != null )
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }
}
