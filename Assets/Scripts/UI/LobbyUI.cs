using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private GameObject createLobbyUI;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private TMP_InputField lobbyCodeInputField;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private Transform lobbySelectorsContainer;
    [SerializeField] private LobbySelectorUI lobbySelectorTemplate;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        createLobbyButton.onClick.AddListener(() =>
        {
            createLobbyUI.SetActive(true);
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.QuickJoin();
        });
        joinCodeButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinWithCode(lobbyCodeInputField.text);
        });
    }

    private void Start()
    {
        playerNameInputField.text = MultiplayerManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string value) =>
        {
            MultiplayerManager.Instance.SetPlayerName(value);
        });
        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        string selectedLobbyId = "";
        foreach (LobbySelectorUI lobbySelectorUI in lobbySelectorsContainer.GetComponentsInChildren<LobbySelectorUI>(false))
        {
            if (EventSystem.current.currentSelectedGameObject == lobbySelectorUI.GetButton().gameObject)
            {
                selectedLobbyId = lobbySelectorUI.GetLobby().Id;
            }
            Destroy(lobbySelectorUI.gameObject);
        }
        bool hasSelected = false;
        foreach (Lobby lobby in e.lobbies)
        {
            LobbySelectorUI lobbySelectorUI = Instantiate(lobbySelectorTemplate, lobbySelectorsContainer);
            lobbySelectorUI.SetLobby(lobby);
            if (lobby.Id == selectedLobbyId)
            {
                EventSystem.current.SetSelectedGameObject(lobbySelectorUI.GetButton().gameObject);
                hasSelected = true;
            }
            lobbySelectorUI.gameObject.SetActive(true);
        }
        if (!hasSelected && selectedLobbyId != "")
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
    }
}
