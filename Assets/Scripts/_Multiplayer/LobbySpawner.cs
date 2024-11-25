using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LobbySpawner : MonoBehaviour
{
    [SerializeField] private List<LobbyPlayer> _players;

    private void OnEnable()
    {
        Game.Events.LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
    }

    private void OnDisable()
    {
        Game.Events.LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;

    }

    private void OnLobbyUpdated()
    {
        List<LobbyPlayerData> playerDatas = GameLobbyManager.Instance.GetPlayers();
        for (int i = 0; i < playerDatas.Count; i++)
        {
            LobbyPlayerData data = playerDatas[i];
            _players[i].SetData(data);
        }
    }
}
