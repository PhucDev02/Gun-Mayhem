using System;
using UnityEngine;

public class GameConfig
{
    public static GameConfigSO data;
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        if (data == null)
            data = Resources.Load<GameConfigSO>("GameConfig");
        Debug.Log("Gameconfig: " + data == null);
    }
}
