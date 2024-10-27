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


    public static EPlayer Winner { get => winner; set => winner = value; }

    bool isSetResult = false;
    public void SetupGameResult()
    {
        if (isSetResult) return;
        isSetResult = true;

        winner = (players[0].CurrentLives > players[1].CurrentLives) ?
            EPlayer.BluePlayer : EPlayer.RedPlayer;
        ShowResult();
    }

    private void ShowResult()
    {
        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.ResultScene);
    }
}
