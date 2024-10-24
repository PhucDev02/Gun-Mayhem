using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Winner
{
    BluePlayer,
    RedPlayer
}

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private static Winner winner;
    [SerializeField] private PlayerHealth[] players;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static Winner Winner { get => winner; set => winner = value; }

    bool isSetResult = false;
    public void SetupGameResult()
    {
        if (isSetResult) return;
        isSetResult = true;

        winner = (players[0].CurrentHealth > players[1].CurrentHealth) ?
            Winner.BluePlayer : Winner.RedPlayer;
        ShowResult();
    }

    private void ShowResult()
    {
        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.ResultScene);
    }
}
