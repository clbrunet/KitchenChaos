using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private Button readyButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        readyButton.onClick.AddListener(() =>
        {
            ReadyClientsManager.Instance.SetClientReady();
        });
    }

    private void Start()
    {
        Lobby lobby = LobbyManager.Instance.GetLobby();
        lobbyNameText.text = "Lobby name : " + lobby.Name;
        lobbyCodeText.text = "Lobby code : " + lobby.LobbyCode;
    }
}
