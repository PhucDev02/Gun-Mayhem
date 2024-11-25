using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyPlayerData
{
    private string _id;
    private string _gameTag;
    private bool _isReady;

    public string Id { get => _id; set => _id = value; }
    public string GameTag { get => _gameTag; set => _gameTag = value; }
    public bool IsReady { get => _isReady; set => _isReady = value; }

    public void Initialize(string id, string gameTag)
    {
        _id = id;
        _gameTag = gameTag;
    }

    public void Initialize(Dictionary<string, PlayerDataObject> playerData) { 
        UpdateState(playerData);
    }

    public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
    {
        if(playerData.ContainsKey("Id"))
        {
            _id = playerData["Id"].Value;
        }
        if (playerData.ContainsKey("GameTag"))
        {
            _gameTag = playerData["GameTag"].Value;
        }
        if (playerData.ContainsKey("IsReady"))
        {
            _isReady = playerData["IsReady"].Value.Equals("True");
        }

    }

    public Dictionary<string, string> Serialize()
    {
        return new Dictionary<string, string>()
        {
            {"Id", _id },
            {"Gametag", _gameTag},
            {"IsReady", _isReady.ToString()},
        };
    }
}
