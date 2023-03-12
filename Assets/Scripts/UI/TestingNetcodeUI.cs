using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartHost();
            gameObject.SetActive(false);
        });
        startClientButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartClient();
            gameObject.SetActive(false);
        });
    }
}
