using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {
        image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.ChangePlayerColor(colorId);
            UpdateSelection();
        });
    }

    private void Start()
    {
        image.color = MultiplayerManager.Instance.GetPlayerSelectableColor(colorId);
        MultiplayerManager.Instance.OnPlayerDatasChanged += MultiplayerManager_OnPlayerDatasChanged;
        UpdateSelection();
    }

    private void MultiplayerManager_OnPlayerDatasChanged(object sender, System.EventArgs e)
    {
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        selectedGameObject.SetActive(colorId == MultiplayerManager.Instance.GetPlayerData().colorId);
    }
}
