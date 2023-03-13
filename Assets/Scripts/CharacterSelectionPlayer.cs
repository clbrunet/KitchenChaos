using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterSelectionPlayer : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private GameObject readyText;

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDatasChanged += MultiplayerManager_OnPlayerDatasChanged;
        ReadyClientsManager.Instance.OnReadyClientsChanged += ReadyClientsManager_OnReadyClientsChanged;
        SetActive();
    }

    private void OnDestroy()
    {
        // TODO: try without it and make the event invoke (for learning purpose)
        print("TODO: try without it and make the event invoke (for learning purpose)");
        MultiplayerManager.Instance.OnPlayerDatasChanged -= MultiplayerManager_OnPlayerDatasChanged;
    }

    private void MultiplayerManager_OnPlayerDatasChanged(object sender, System.EventArgs e)
    {
        SetActive();
    }

    private void SetActive()
    {
        gameObject.SetActive(MultiplayerManager.Instance.IsIndexConnected(index));
    }

    private void ReadyClientsManager_OnReadyClientsChanged(object sender, System.EventArgs e)
    {
        if (!MultiplayerManager.Instance.IsIndexConnected(index))
        {
            return;
        }
        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromIndex(index);
        readyText.SetActive(ReadyClientsManager.Instance.IsClientReady(playerData.clientId));
    }
}
