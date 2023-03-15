using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    private const string LOBBY_DATA_RELAY_JOIN_CODE = "RelayJoinCode";

    private bool isCreatingLobby = false;
    private Lobby joinedLobby = null;

    public event EventHandler OnLobbyCreationStarted;
    public event EventHandler OnLobbyCreationFailed;

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbies;

        public OnLobbyListChangedEventArgs(List<Lobby> lobbies)
        {
            this.lobbies = lobbies;
        }
    }

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
            AuthenticationService_SignedIn();
            return;
        }
        InitializationOptions initializationOptions = new();
        initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
        await UnityServices.InitializeAsync(initializationOptions);
        AuthenticationService.Instance.SignedIn += AuthenticationService_SignedIn;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void OnDestroy()
    {
        if (AuthenticationService.Instance != null)
        {
            AuthenticationService.Instance.SignedIn -= AuthenticationService_SignedIn;
        }
    }

    private void AuthenticationService_SignedIn()
    {
        const float LOBBIES_REFRESH_INTERVAL = 5f;
        InvokeRepeating(nameof(RefreshLobbies), 0f, LOBBIES_REFRESH_INTERVAL);
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
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerManager.MAX_PLAYER_COUNT);
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { LOBBY_DATA_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                },
            });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            MultiplayerManager.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectionScene);
        }
        catch (RequestFailedException e)
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

    public void RefreshLobbies()
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString() || joinedLobby != null)
        {
            return;
        }
        ListLobbies();
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                },
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs(queryResponse.Results));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = joinedLobby.Data[LOBBY_DATA_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            MultiplayerManager.Instance.StartClient();
        }
        catch (RequestFailedException e)
        {
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }

    public async void JoinWithId(string lobbyId)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayJoinCode = joinedLobby.Data[LOBBY_DATA_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            MultiplayerManager.Instance.StartClient();
        }
        catch (RequestFailedException e)
        {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string relayJoinCode = joinedLobby.Data[LOBBY_DATA_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            MultiplayerManager.Instance.StartClient();
        }
        catch (RequestFailedException e)
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
