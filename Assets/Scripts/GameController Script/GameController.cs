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

[DefaultExecutionOrder(-1)]
public class GameController : Singleton<GameController>
{
    private static EPlayer winner;
    public static EPlayer Winner { get => winner; set => winner = value; }
    [SerializeField] private List<PlayerController> players = new List<PlayerController>();

    Vector3 midPoint;

    public void RegisterPlayer(PlayerController p)
    {
        if (!players.Contains(p))
            players.Add(p);
    }

    public void UnRegisterPlayer(PlayerController p)
    {
        if(players.Contains(p))
            players.Remove(p);
    }

    public override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 120;
    }

    private void Update()
    {
        if (players.Count != 2) return;
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

        winner = (players[0].playerLives.CurrentLives > players[1].playerLives.CurrentLives) ?
            players[0].EPlayer : players[1].EPlayer;
        Invoke(nameof(ShowResult), 1f);
    }

    private void ShowResult()
    {
        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.ResultScene);
    }

    public int GetTotalPlayer()
    {
        return players.Count;
    }
}
