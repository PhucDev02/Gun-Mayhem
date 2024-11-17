using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EPlayer
{
    BluePlayer,
    RedPlayer,
    AI,
    Dummy
}

[DefaultExecutionOrder(-1)]
public class GameController : Singleton<GameController>
{
    private static EPlayer winner;
    public Actor[] players;

    Vector3 midPoint;

    public override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 120;
        players = FindObjectsByType<Actor>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        midPoint = Vector3.zero;
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i] != null)
            {
                midPoint += players[i].transform.position;
            }
        }
        midPoint/=players.Count;
        transform.position = midPoint;
        UpdateMeanPlayersDistance();
    }
    private void UpdateMeanPlayersDistance()
    {
        CameraController.meanDistancePlayers = Vector3.Distance(players[0].transform.position, players[1].transform.position);
    }

    bool isSetResult = false;
    public void SetupGameResult()
    {
        if (isSetResult) return;
        isSetResult = true;

        winner = (players[0].life.CurrentLives > players[1].life.CurrentLives) ?
            EPlayer.BluePlayer : EPlayer.RedPlayer;
        Invoke(nameof(ShowResult), 1f);
    }

    private void ShowResult()
    {
        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.ResultScene);
    }
    public Vector2 GetTargetPosition()
    {
        foreach(var x in players)
        {
            if (x.type != EPlayer.AI)
                return (Vector2) x.transform.position;
        }
        return Vector2.zero;
    }
}
