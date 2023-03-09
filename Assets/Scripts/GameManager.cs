using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }
    private State state = State.WaitingToStart;

    private float waitingToStartTimer = 0.3f;
    private float countdownToStartTimer = 3f;
    [SerializeField] private float gamePlayingTimerMax = 10f;
    private float gamePlayingTimer;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;

        public OnStateChangedEventArgs(State state)
        {
            this.state = state;
        }
    }

    private bool isGamePaused = false;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of GameManager");
        Instance = this;
        gamePlayingTimer = gamePlayingTimerMax;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer <= 0f)
                {
                    SetState(State.CountdownToStart);
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer <= 0f)
                {
                    SetState(State.GamePlaying);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0f)
                {
                    SetState(State.GameOver);
                }
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs(state));
    }

    public bool IsPlaying()
    {
        return state == State.GamePlaying;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public float GetNormalizedGamePlayingTimer()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }
}
