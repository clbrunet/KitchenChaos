using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        multiplayerButton.onClick.AddListener(() =>
        {
            MultiplayerManager.isSingleplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        singleplayerButton.onClick.AddListener(() =>
        {
            MultiplayerManager.isSingleplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
