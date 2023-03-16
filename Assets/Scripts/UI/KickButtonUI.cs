using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KickButtonUI : MonoBehaviour
{
    private Button kickButton;
    [SerializeField] private int index;

    private void Awake()
    {
        kickButton = GetComponent<Button>();
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromIndex(index);
            MultiplayerManager.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        gameObject.SetActive(NetworkManager.Singleton.IsServer && index != 0 && MultiplayerManager.Instance.IsIndexConnected(index));
        MultiplayerManager.Instance.OnPlayerDatasChanged += MultiplayerManager_OnPlayerDatasChanged;
    }

    private void OnDisable()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnPlayerDatasChanged -= MultiplayerManager_OnPlayerDatasChanged;
    }

    private void MultiplayerManager_OnPlayerDatasChanged(object sender, System.EventArgs e)
    {
        gameObject.SetActive(NetworkManager.Singleton.IsServer && index != 0 && MultiplayerManager.Instance.IsIndexConnected(index));
    }
}
