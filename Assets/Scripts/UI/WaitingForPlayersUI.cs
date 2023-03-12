using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnIsLocalPlayerReadyChanged += GameManager_OnIsLocalPlayerReadyChanged;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        GameManager.Instance.OnHostDisconnected += GameManager_OnHostDisconnected;
        gameObject.SetActive(false);
    }

    private void GameManager_OnIsLocalPlayerReadyChanged(object sender, System.EventArgs e)
    {
        gameObject.SetActive(GameManager.Instance.IsLocalPlayerReady());
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e)
    {
        if (e.state == GameManager.State.CountdownToStart)
        {
            gameObject.SetActive(false);
        }
    }

    private void GameManager_OnHostDisconnected(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }
}
