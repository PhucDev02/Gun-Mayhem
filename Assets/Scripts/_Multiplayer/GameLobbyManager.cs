using Events;
using Multiplayer.Manager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections;
using Unity.Services.Lobbies;
using Unity.VisualScripting;

public class GameLobbyManager : Singleton<GameLobbyManager>
{
    public List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();
    private LobbyPlayerData _localLobbyPlayerData;
    private LobbyData _lobbyData;
    private int _maxNumberOfPlayer = 2;
    private bool _ingame = false;

    public bool IsHost
    {
        get
        {
            return _localLobbyPlayerData.Id.Equals(LobbyManager.Instance.GetHostId());
        }
    }

    private void OnEnable()
    {
        Events.LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;    
    }


    private void OnDisable()
    {
        Events.LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;

    }
    private async void OnLobbyUpdated(Lobby lobby)
    {
        List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.Instance.GetPlayersData();
        _lobbyPlayerDatas.Clear();

        int numberOfPlayerReady = 0;

        foreach (var item in playerData)
        {
            LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
            lobbyPlayerData.Initialize(item);

            if(lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId)
            {
                _localLobbyPlayerData = lobbyPlayerData;
            }

            if(lobbyPlayerData.IsReady) numberOfPlayerReady++;

            _lobbyPlayerDatas.Add(lobbyPlayerData);
        }

        _lobbyData = new LobbyData();
        _lobbyData.Initialize(lobby.Data);
        Game.Events.LobbyEvents.OnLobbyUpdated?.Invoke();

        if(numberOfPlayerReady == lobby.Players.Count)
        {
            Game.Events.LobbyEvents.OnLobbyReady?.Invoke();
        }

        if(_lobbyData.RelayJoinCode != default && !RelayManager.Instance.IsHost && !_ingame)
        {
            //Join the relay
            await JoinRelayServer(_lobbyData.RelayJoinCode);
            MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.MultiplayerScene);
        }
    }

    private async Task<bool> JoinRelayServer(string relayJoinCode)
    {
        _ingame = true;
        await RelayManager.Instance.JoinRelay(relayJoinCode);
        string allocationId = RelayManager.Instance.GetAllocationId();
        string connectionData = RelayManager.Instance.GetConnectionData();
        await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);
        return true;
    }

    public async Task<bool> CreateLobby()
    {
        _localLobbyPlayerData = new LobbyPlayerData();
        _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, "HostPlayer");
        _lobbyData = new LobbyData();
        bool succeeded =  await LobbyManager.Instance.CreateLobby(_maxNumberOfPlayer, true, _localLobbyPlayerData.Serialize(), _lobbyData.Serialize());
        return succeeded;
    }

    public async Task<bool> JoinLobby(string code)
    {
        _localLobbyPlayerData = new LobbyPlayerData();
        _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, "JoinPlayer");
        bool succeed = await LobbyManager.Instance.JoinLobby(code, _localLobbyPlayerData.Serialize());
        return succeed;

    }

    public string GetLobbyCode()
    {
        string lobbyCode = LobbyManager.Instance.GetLobbyCode();
        Debug.LogError("Lobby code: " + lobbyCode);
        return lobbyCode;
    }

    public List<LobbyPlayerData> GetPlayers()
    {
        return _lobbyPlayerDatas;
    }

    public async Task<bool> SetPlayerReady()
    {
        _localLobbyPlayerData.IsReady = true;
        return await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize());
    }

    public async Task StartGame()
    {
        _ingame = true;
        string relayJoinCode = await RelayManager.Instance.CreateRelay(_maxNumberOfPlayer);
        _lobbyData.RelayJoinCode = relayJoinCode;
        await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());

        string allocationId = RelayManager.Instance.GetAllocationId();
        string connectionData = RelayManager.Instance.GetConnectionData();

        await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);

        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.MultiplayerScene);
    }
}
