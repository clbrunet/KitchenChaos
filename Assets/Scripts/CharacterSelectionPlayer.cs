using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionPlayer : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private TextMeshPro playerNameText;
    [SerializeField] private GameObject readyText;
    [SerializeField] private PlayerVisual playerVisual;

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
        playerNameText.text = playerData.name.ToString();
        readyText.SetActive(ReadyClientsManager.Instance.IsClientReady(playerData.clientId));
        playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerSelectableColor(playerData.colorId));
    }
}
