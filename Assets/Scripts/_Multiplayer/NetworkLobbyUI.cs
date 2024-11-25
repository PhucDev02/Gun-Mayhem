using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static Game.Events.LobbyEvents;

public class NetworkLobbyUI : MonoBehaviour
{
    [SerializeField] private Button _readyBtn;
    [SerializeField] private Button _startBtn;

    private void OnEnable()
    {
        Game.Events.LobbyEvents.OnLobbyUpdated += OnLobbyUpdate;
        _readyBtn.onClick.AddListener(OnReadyPress);

        //_startBtn.onClick.AddListener(OnStartBtnClick);
        //Game.Events.LobbyEvents.OnLobbyReady += OnLobbyReady;

        if (GameLobbyManager.Instance.IsHost)
        {
            Game.Events.LobbyEvents.OnLobbyReady += OnLobbyReady;
            _startBtn.onClick.AddListener(OnStartBtnClick);
        }
    }

    private void OnDisable()
    {
        _readyBtn.onClick?.RemoveListener(OnReadyPress);
        _startBtn.onClick.RemoveListener(OnStartBtnClick);

        Game.Events.LobbyEvents.OnLobbyReady -= OnLobbyReady;
        Game.Events.LobbyEvents.OnLobbyUpdated -= OnLobbyUpdate;

    }

    private void OnLobbyUpdate()
    {
        //throw new NotImplementedException();
    }

    private async void OnReadyPress()
    {
        bool succeed = await GameLobbyManager.Instance.SetPlayerReady();
        if (succeed)
        {
            _readyBtn.gameObject.SetActive(false);
        }
    }

    private void OnLobbyReady()
    { 
        _startBtn.gameObject.SetActive(true);
    }

    private async void OnStartBtnClick()
    {
        await GameLobbyManager.Instance.StartGame();
    }


    //void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(10, 10, 300, 300));
    //    if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
    //    {
    //        StartButtons();
    //    }
    //    else
    //    {
    //        StatusLabels();
    //    }
    //    GUILayout.EndArea();
    //}

    //static void StartButtons()
    //{
    //    if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
    //    if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
    //    if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    //}

    //static void StatusLabels()
    //{
    //    var mode = NetworkManager.Singleton.IsHost ?
    //        "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
    //    GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
    //    GUILayout.Label("Mode: " + mode);
    //}

}
