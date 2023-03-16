using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private HostDisconnectedUI hostDisconnectedUI;
    [SerializeField] private GameOverUI gameOverUI;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePause();
        });
        optionsButton.onClick.AddListener(() =>
        {
            OptionsUI.Instance.Open(() =>
            {
                gameObject.SetActive(true);
                optionsButton.Select();
            });
            gameObject.SetActive(false);
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnLocalPlayerPaused += GameManager_OnLocalPlayerPaused;
        GameManager.Instance.OnLocalPlayerUnpaused += GameManager_OnLocalPlayerUnpaused;
        gameObject.SetActive(false);
    }

    private void GameManager_OnLocalPlayerPaused(object sender, System.EventArgs e)
    {
        hostDisconnectedUI.shouldAppear = false;
        gameOverUI.shouldAppear = false;
        if (GameManager.Instance.IsHostDisconnected())
        {
            hostDisconnectedUI.gameObject.SetActive(false);
        }
        if (GameManager.Instance.IsOver())
        {
            gameOverUI.gameObject.SetActive(false);
        }
        gameObject.SetActive(true);
        resumeButton.Select();
    }

    private void GameManager_OnLocalPlayerUnpaused(object sender, System.EventArgs e)
    {
        gameOverUI.shouldAppear = true;
        gameObject.SetActive(false);
        if (GameManager.Instance.IsOver())
        {
            gameOverUI.gameObject.SetActive(true);
            return;
        }
        hostDisconnectedUI.shouldAppear = true;
        if (GameManager.Instance.IsHostDisconnected())
        {
            hostDisconnectedUI.gameObject.SetActive(true);
        }
    }
}
