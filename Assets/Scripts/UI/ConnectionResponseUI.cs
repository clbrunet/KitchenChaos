using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseUI : MonoBehaviour
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
        MultiplayerManager.Instance.OnFailedToJoin += MultiplayerManager_OnFailedToJoin;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnFailedToJoin -= MultiplayerManager_OnFailedToJoin;
    }

    private void MultiplayerManager_OnFailedToJoin(object sender, System.EventArgs e)
    {
        messageText.text = NetworkManager.Singleton.DisconnectReason;
        if (messageText.text == "")
        {
            messageText.text = "Failed to connect";
        }
        gameObject.SetActive(true);
    }
}
