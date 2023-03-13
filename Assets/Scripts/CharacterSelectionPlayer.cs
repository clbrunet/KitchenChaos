using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterSelectionPlayer : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private GameObject readyText;

    private void Awake()
    {
        playerVisual = GetComponentInChildren<PlayerVisual>();
    }

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDatasChanged += MultiplayerManager_OnPlayerDatasChanged;
        ReadyClientsManager.Instance.OnReadyClientsChanged += ReadyClientsManager_OnReadyClientsChanged;
        UpdateVisual();
    }

    private void OnDestroy()
    {
        // TODO: try without it and make the event invoke (for learning purpose)
        print("TODO: try without it and make the event invoke (for learning purpose)");
        MultiplayerManager.Instance.OnPlayerDatasChanged -= MultiplayerManager_OnPlayerDatasChanged;
    }

    private void MultiplayerManager_OnPlayerDatasChanged(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void ReadyClientsManager_OnReadyClientsChanged(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (!MultiplayerManager.Instance.IsIndexConnected(index))
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromIndex(index);
        playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerSelectableColor(playerData.colorId));
        readyText.SetActive(ReadyClientsManager.Instance.IsClientReady(playerData.clientId));
    }
}
