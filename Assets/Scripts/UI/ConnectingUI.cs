using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        MultiplayerManager.Instance.OnTryingToJoin += MultiplayerManager_OnTryingToJoin;
        MultiplayerManager.Instance.OnFailedToJoin += MultiplayerManager_OnFailedToJoin;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnTryingToJoin -= MultiplayerManager_OnTryingToJoin;
        MultiplayerManager.Instance.OnFailedToJoin -= MultiplayerManager_OnFailedToJoin;
    }

    private void MultiplayerManager_OnTryingToJoin(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void MultiplayerManager_OnFailedToJoin(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }
}
