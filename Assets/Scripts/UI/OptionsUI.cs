using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsVolumeButton;
    [SerializeField] private Button musicVolumeButton;
    [SerializeField] private TextMeshProUGUI soundEffectsVolumeButtonText;
    [SerializeField] private TextMeshProUGUI musicVolumeButtonText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of OptionsUI");
        Instance = this;
        soundEffectsVolumeButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicVolumeButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;
        UpdateVisual();
        gameObject.SetActive(false);
    }

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        soundEffectsVolumeButtonText.text = "Sound effects volume : " + Mathf.RoundToInt(SoundManager.Instance.GetVolume() * 10);
        musicVolumeButtonText.text = "Music volume : " + Mathf.RoundToInt(MusicManager.Instance.GetVolume() * 10);
    }
}
