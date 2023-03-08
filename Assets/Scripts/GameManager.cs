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
    private float gamePlayingTimerMax = 10f;
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

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of GameManager");
        Instance = this;
        gamePlayingTimer = gamePlayingTimerMax;
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
