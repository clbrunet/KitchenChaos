using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbySelectorUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI lobbyNameText;

    private Lobby lobby;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinWithId(lobby.Id);
        });
    }

    public Lobby GetLobby()
    {
        return lobby;
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }

    public Button GetButton()
    {
        return button;
    }
}
