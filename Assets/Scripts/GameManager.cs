using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }
    private readonly NetworkVariable<State> state = new(State.WaitingToStart);
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;

        public OnStateChangedEventArgs(State state)
        {
            this.state = state;
        }
    }
    private bool isHostDisconnected = false;
    private bool isLocalPlayerReady = false;
    public event EventHandler OnIsLocalPlayerReadyChanged;
    private readonly Dictionary<ulong, bool> serverReadyClientsDictionary = new();

    private readonly NetworkVariable<float> countdownToStartTimer = new(3f);
    [SerializeField] private float gamePlayingTimerMax = 90f;
    private NetworkVariable<float> gamePlayingTimer;

    private readonly Dictionary<ulong, bool> serverPausedClientsDictionary = new();
    private bool isLocalPlayerPaused = false;
    public event EventHandler OnLocalPlayerPaused;
    public event EventHandler OnLocalPlayerUnpaused;
    private readonly NetworkVariable<bool> isGamePaused = new(false);
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of GameManager");
        Instance = this;
        gamePlayingTimer = new(gamePlayingTimerMax);
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_ServerOnClientDisconnectCallback;
        }
        else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_ClientOnClientDisconnectCallback;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_ServerOnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_ClientOnClientDisconnectCallback;
        }
    }

    private void NetworkManager_ServerOnClientDisconnectCallback(ulong clientId)
    {
        serverPausedClientsDictionary[clientId] = false;
        SetIsGamePaused();
    }

    private void NetworkManager_ClientOnClientDisconnectCallback(ulong clientId)
    {
        if (clientId != NetworkManager.ServerClientId)
        {
            return;
        }
        isHostDisconnected = true;
        IsGamePaused_OnValueChanged(isGamePaused.Value, false);
    }


    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            SetIsLocalPlayerReady(true);
        }
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value <= 0f)
                {
                    SetState(State.GamePlaying);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value <= 0f)
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
        this.state.Value = state;
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs(newValue));
    }

    private void SetIsLocalPlayerReady(bool isLocalPlayerReady)
    {
        this.isLocalPlayerReady = isLocalPlayerReady;
        OnIsLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
        SetPlayerReadyServerRpc(isLocalPlayerReady);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(bool isPlayerReady, ServerRpcParams serverRpcParams = default)
    {
        serverReadyClientsDictionary[serverRpcParams.Receive.SenderClientId] = isPlayerReady;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!serverReadyClientsDictionary.ContainsKey(clientId)
                || !serverReadyClientsDictionary[clientId])
            {
                return;
            }
        }
        SetState(State.CountdownToStart);
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        isLocalPlayerPaused = !isLocalPlayerPaused;
        TogglePauseServerRpc(isLocalPlayerPaused);
        if (isLocalPlayerPaused)
        {
            OnLocalPlayerPaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnLocalPlayerUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TogglePauseServerRpc(bool isPaused, ServerRpcParams serverRpcParams = default)
    {
        serverPausedClientsDictionary[serverRpcParams.Receive.SenderClientId] = isPaused;
        SetIsGamePaused();
    }

    private void SetIsGamePaused()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (serverPausedClientsDictionary.ContainsKey(clientId)
                && serverPausedClientsDictionary[clientId])
            {
                isGamePaused.Value = true;
                return;
            }
        }
        isGamePaused.Value = false;
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (newValue)
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

    public bool IsHostDisconnected()
    {
        return isHostDisconnected;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public bool IsPlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsOver()
    {
        return state.Value == State.GameOver;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public float GetNormalizedGamePlayingTimer()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
}
