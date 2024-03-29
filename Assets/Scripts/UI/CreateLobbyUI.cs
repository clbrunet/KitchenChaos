using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;

    private bool hasStarted = false;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        });
        createPublicButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, false);
        });
        createPrivateButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, true);
        });
    }

    private void OnEnable()
    {
        if (!hasStarted)
        {
            return;
        }
        createPublicButton.Select();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        hasStarted = true;
    }
}
