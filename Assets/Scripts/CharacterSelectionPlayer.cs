using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionPlayer : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private GameObject readyText;
    [SerializeField] private Button kickButton;

    private void Awake()
    {
        playerVisual = GetComponentInChildren<PlayerVisual>();
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromIndex(index);
            MultiplayerManager.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && index != 0);
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
        playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerSelectableColor(playerData.colorId));
        readyText.SetActive(ReadyClientsManager.Instance.IsClientReady(playerData.clientId));
    }
}
