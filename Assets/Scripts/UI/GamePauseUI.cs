using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private GameObject gameOverUI;

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
        if (GameManager.Instance.IsOver())
        {
            gameOverUI.SetActive(false);
        }
        gameObject.SetActive(true);
        resumeButton.Select();
    }

    private void GameManager_OnLocalPlayerUnpaused(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
        if (GameManager.Instance.IsOver())
        {
            gameOverUI.SetActive(true);
        }
    }
}
