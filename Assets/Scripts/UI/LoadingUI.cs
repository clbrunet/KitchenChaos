using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(MultiplayerManager.isSingleplayer);
    }
}
