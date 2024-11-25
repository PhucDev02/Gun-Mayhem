using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Multiplayer.Manager
{
    public class LobbyManager: Singleton<LobbyManager>
    {
        private Lobby _lobby;
        private Coroutine _hearthbeatCoroutine;
        private Coroutine _refreshLobbyCoroutine;

        public async Task<bool> CreateLobby(int maxPlayer, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                Data = SerializeLobbyData(lobbyData),
                IsPrivate = isPrivate,
                Player = player,
            };
            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync("lobby", maxPlayer, options);

            }
            catch (Exception)
            {

                return false;
            }
            _hearthbeatCoroutine = StartCoroutine(HearthbeatLobbyCoroutine(_lobby.Id, 6f));
            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
            return true;
        }

        public async Task<bool> JoinLobby(string joinCode, Dictionary<string,string> playerData)
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(playerData));
            options.Player = player;

            try
            {
                _lobby =  await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode, options);
            }
            catch (Exception)
            {

                return false;
            }
            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
            return true;

        }

        private void StopLobbyCoroutine()
        {
            if(_hearthbeatCoroutine != null) StopCoroutine(_hearthbeatCoroutine);
            if(_refreshLobbyCoroutine != null) StopCoroutine(_refreshLobbyCoroutine);   
        }

        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode;
        }

        private IEnumerator HearthbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Debug.Log("HearthBeat");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);
                Lobby newLobby = task.Result;
                if(newLobby.LastUpdated > _lobby.LastUpdated)
                {
                    _lobby = newLobby;
                    Events.LobbyEvents.OnLobbyUpdated?.Invoke(_lobby);
                }
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();
            foreach (var item in data)
            {
                playerData.Add(item.Key, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Member,
                    value: item.Value));
            }
            return playerData;
        }

        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
            foreach (var item in data)
            {
                lobbyData.Add(item.Key, new DataObject(visibility: DataObject.VisibilityOptions.Member,
                    value: item.Value));
            }
            return lobbyData;
        }

        private void OnApplicationQuit()
        {
            DeleteLobby();
        }

        public void DeleteLobby()
        {
            StopLobbyCoroutine();
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            }
        }

        public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
        {
            List<Dictionary<string, PlayerDataObject>> data = new List<Dictionary<string, PlayerDataObject>>();
            foreach (var item in _lobby.Players)
            {
                data.Add(item.Data);
            }
            return data;
        }

        public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocationId = default, string connectionData = default)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

            UpdatePlayerOptions options = new UpdatePlayerOptions()
            {
                Data = playerData,
                AllocationId = allocationId,
                ConnectionInfo = connectionData
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);
            }
            catch (System.Exception)
            {
                return false;
            }

            Events.LobbyEvents.OnLobbyUpdated(_lobby);

            return true;
        }


        public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = SerializeLobbyData(data);

            UpdateLobbyOptions options = new UpdateLobbyOptions()
            {
                Data = lobbyData
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);
            }
            catch (System.Exception)
            {
                return false;
            }

            Events.LobbyEvents.OnLobbyUpdated(_lobby);

            return true;
        }


        public string GetHostId()
        {
            return _lobby.HostId;
        }
    }
}