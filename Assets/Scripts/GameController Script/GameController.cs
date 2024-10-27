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
    [SerializeField] private PlayerHealth[] players;

    Vector3 midPoint;
    private void Update()
    {
        midPoint = Vector3.zero;
        for(int i = 0; i < players.Length; i++)
        {
            midPoint += players[i].transform.position;
        }
        midPoint/=players.Length;
        transform.position = midPoint;
        UpdateMeanPlayersDistance();
    }
    private void UpdateMeanPlayersDistance()
    {
        CameraController.meanDistancePlayers = Vector3.Distance(players[0].transform.position, players[1].transform.position)/2;
    }
    public static EPlayer Winner { get => winner; set => winner = value; }

    bool isSetResult = false;
    public void SetupGameResult()
    {
        if (isSetResult) return;
        isSetResult = true;

        winner = (players[0].CurrentHealth > players[1].CurrentHealth) ?
            EPlayer.BluePlayer : EPlayer.RedPlayer;
        ShowResult();
    }

    private void ShowResult()
    {
        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.ResultScene);
    }
}
