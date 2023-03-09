using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsVolumeButton;
    [SerializeField] private Button musicVolumeButton;
    [SerializeField] private TextMeshProUGUI soundEffectsVolumeButtonText;
    [SerializeField] private TextMeshProUGUI musicVolumeButtonText;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private TextMeshProUGUI moveUpButtonText;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private TextMeshProUGUI moveDownButtonText;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private TextMeshProUGUI moveLeftButtonText;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private TextMeshProUGUI moveRightButtonText;
    [SerializeField] private Button interactButton;
    [SerializeField] private TextMeshProUGUI interactButtonText;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private TextMeshProUGUI interactAltButtonText;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject rebind;

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
        moveUpButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.MoveUp);
        });
        moveDownButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.MoveDown);
        });
        moveLeftButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.MoveLeft);
        });
        moveRightButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.MoveRight);
        });
        interactButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Interact);
        });
        interactAltButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.InteractAlternate);
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
        moveUpButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        moveDownButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        interactButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAltButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        rebind.SetActive(true);
        GameInput.Instance.RebindBinding(binding, () =>
        {
            UpdateVisual();
            rebind.SetActive(false);
        });
    }
}
