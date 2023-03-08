using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool isFirstUpdateDone = false;

    void Update()
    {
        if (!isFirstUpdateDone)
        {
            Loader.LoaderCallback();
            isFirstUpdateDone = true;
        }
    }
}
