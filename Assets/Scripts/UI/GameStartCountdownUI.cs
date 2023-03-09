using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Animator animator;
    private const string NUMBER_POPUP = "NumberPopup";

    int previous = -1;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        int current = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
        text.text = current.ToString();
        if (current != previous)
        {
            previous = current;
            animator.SetTrigger(NUMBER_POPUP);
            SoundManager.Instance.PlayWarning(Vector3.zero);
        }
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e)
    {
        gameObject.SetActive(e.state == GameManager.State.CountdownToStart);
    }
}
