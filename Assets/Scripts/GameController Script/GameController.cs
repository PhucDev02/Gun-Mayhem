using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EPlayer
{
    BluePlayer,
    RedPlayer
}

public class GameController : Singleton<GameController>
{
    private static EPlayer winner;
    [SerializeField] private PlayerLives[] players;

    Vector3 midPoint;

    public override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 120;
    }

    private void Update()
    {
        midPoint = Vector3.zero;
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                midPoint += players[i].transform.position;
            }
        }
        midPoint/=players.Length;
        transform.position = midPoint;
        UpdateMeanPlayersDistance();
    }
    private void UpdateMeanPlayersDistance()
    {
        CameraController.meanDistancePlayers = Vector3.Distance(players[0].transform.position, players[1].transform.position);
    }
    public static EPlayer Winner { get => winner; set => winner = value; }

    bool isSetResult = false;
    public void SetupGameResult()
    {
        if (isSetResult) return;
        isSetResult = true;

        winner = (players[0].CurrentLives > players[1].CurrentLives) ?
            EPlayer.BluePlayer : EPlayer.RedPlayer;
        Invoke(nameof(ShowResult), 1f);
    }

    private void ShowResult()
    {
        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.ResultScene);
    }
}
