using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private Animator animator;
    private const string POPUP = "Popup";

    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;

    [SerializeField] private Color successColor;
    [SerializeField] private Color failColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnDeliverySuccess += DeliveryManager_OnDeliverySuccess;
        DeliveryManager.Instance.OnDeliveryFail += DeliveryManager_OnDeliveryFail;
        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnDeliverySuccess(object sender, System.EventArgs e)
    {
        background.color = successColor;
        text.text = "DELIVERY\nSUCCESS";
        icon.sprite = successSprite;
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP);
    }

    private void DeliveryManager_OnDeliveryFail(object sender, System.EventArgs e)
    {
        background.color = failColor;
        text.text = "DELIVERY\nFAIL";
        icon.sprite = failSprite;
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP);
    }
}
