using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Events
{
    public static class LobbyEvents
    {
        public delegate void LobbyUpdated(Lobby lobby);
        public static LobbyUpdated OnLobbyUpdated;

    }
}