using System;
using UnityEngine;

public class Config
{
    public static GameConfigSO data;
    public static PlayerConfigSO player;
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        if (data == null)
        {
            data = Resources.Load<GameConfigSO>("GameConfig");
        }
        if(player == null)
        {
            player = Resources.Load<PlayerConfigSO>("PlayerConfig");
        }
        //Debug.Log("Gameconfig: " + data == null);
    }
}
