using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void Start()
    {
        LobbyManager.Instance.OnLobbyCreationStarted += LobbyManager_OnLobbyCreationStarted;
        LobbyManager.Instance.OnLobbyCreationFailed += LobbyManager_OnLobbyCreationFailed;
        LobbyManager.Instance.OnJoinStarted += LobbyManager_OnJoinStarted;
        LobbyManager.Instance.OnQuickJoinFailed += LobbyManager_OnQuickJoinFailed;
        LobbyManager.Instance.OnJoinFailed += LobbyManager_OnJoinFailed;
        MultiplayerManager.Instance.OnFailedToJoin += MultiplayerManager_OnFailedToJoin;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyCreationStarted -= LobbyManager_OnLobbyCreationStarted;
        LobbyManager.Instance.OnLobbyCreationFailed -= LobbyManager_OnLobbyCreationFailed;
        LobbyManager.Instance.OnJoinStarted -= LobbyManager_OnJoinStarted;
        LobbyManager.Instance.OnQuickJoinFailed -= LobbyManager_OnQuickJoinFailed;
        LobbyManager.Instance.OnJoinFailed -= LobbyManager_OnJoinFailed;
        MultiplayerManager.Instance.OnFailedToJoin -= MultiplayerManager_OnFailedToJoin;
    }

    private void LobbyManager_OnLobbyCreationStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating lobby...");
    }

    private void LobbyManager_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining lobby...");
    }

    private void LobbyManager_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to find or join a lobby");
    }

    private void LobbyManager_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join the lobby");
    }

    private void LobbyManager_OnLobbyCreationFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create a lobby");
    }

    private void MultiplayerManager_OnFailedToJoin(object sender, System.EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);
    }
}
