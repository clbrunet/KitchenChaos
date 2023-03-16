using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private HostDisconnectedUI hostDisconnectedUI;

    private bool hasStarted = false;
    [HideInInspector] public bool shouldAppear = true;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void OnEnable()
    {
        if (!hasStarted)
        {
            return;
        }
        mainMenuButton.Select();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        gameObject.SetActive(false);
        hasStarted = true;
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e)
    {
        if (e.state == GameManager.State.GameOver)
        {
            if (shouldAppear)
            {
                gameObject.SetActive(true);
            }
            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulDeliveryCount().ToString();
            hostDisconnectedUI.shouldAppear = false;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
