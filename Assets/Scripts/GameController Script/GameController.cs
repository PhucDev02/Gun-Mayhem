using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
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
    public static EPlayer winner;
    public List<Actor> players = new List<Actor>();
    public int GetTotalPlayer => players.Count;


    Vector3 midPoint;

    public override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 120;
        //players = FindObjectsByType<Actor>(FindObjectsSortMode.None);
    }

    public void RegisterActor(Actor actor)
    {
        if(!players.Contains(actor))
            players.Add(actor);
    }

    public void UnRegisterActor(Actor actor)
    {
        if(players.Contains(actor))
            players.Remove(actor);
    }

    private void DeSpawnPlayer()
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i] != null)
            {
                players[i].GetComponent<NetworkObject>().Despawn();
            }
        }
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

        winner = (players[0].life.CurrentLives > players[1].life.CurrentLives) ?
            EPlayer.BluePlayer : EPlayer.RedPlayer;
        StartCoroutine(ShowResult());
    }

    IEnumerator ShowResult()
    {
        yield return new WaitForSeconds(1f);
        if(GameManager.Instance.CurrentGameMode == GameMode.Multiplayer)
        {
            if(RelayManager.Instance.IsHost)
                DeSpawnPlayer();
            yield return new WaitUntil(() => players.Count == 0);
        }
        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.ResultScene);
    }

    [Button]
    public void Test()
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
