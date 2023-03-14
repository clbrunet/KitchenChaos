using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Assertions;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    private bool isCreatingLobby = false;
    private Lobby joinedLobby;

    public event EventHandler OnLobbyCreationStarted;
    public event EventHandler OnLobbyCreationFailed;

    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnQuickJoinFailed;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of LobbyManager");
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitalizeUnityAuthentication();
    }

    private async void InitalizeUnityAuthentication()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            return;
        }
        InitializationOptions initializationOptions = new();
        initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
        await UnityServices.InitializeAsync(initializationOptions);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        if (isCreatingLobby || joinedLobby != null)
        {
            return;
        }
        try
        {
            OnLobbyCreationStarted?.Invoke(this, EventArgs.Empty);
            isCreatingLobby = true;
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.MAX_PLAYER_COUNT, new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            });
            isCreatingLobby = false;
            StartCoroutine(SendHeartbeats());
            MultiplayerManager.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectionScene);
        }
        catch (LobbyServiceException e)
        {
            OnLobbyCreationFailed?.Invoke(this, EventArgs.Empty);
            isCreatingLobby = false;
            Debug.Log(e);
        }
    }

    private IEnumerator SendHeartbeats()
    {
        while (true)
        {
            const float HEARTBEATS_INTERVAL = 15f;
            yield return new WaitForSeconds(HEARTBEATS_INTERVAL);
            if (joinedLobby == null)
            {
                yield break;
            }
            try
            {
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                yield break;
            }
        }
    }

    public async void QuickJoin()
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    public async void LeaveLobby()
    {
        if (joinedLobby == null)
        {
            return;
        }
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (joinedLobby == null || joinedLobby.HostId != AuthenticationService.Instance.PlayerId)
        {
            return;
        }
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby == null)
        {
            return;
        }
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
