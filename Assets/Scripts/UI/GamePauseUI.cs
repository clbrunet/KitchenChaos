using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;

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
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;
        gameObject.SetActive(false);
    }

    private void GameManager_OnGamePaused(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    }

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }
}
