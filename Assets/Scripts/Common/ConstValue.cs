using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;


public class ConstValue
{
    public const int maxLives = 3;
    public static Vector2 environmentLimitX = new Vector2(-10, 10);


    public class Tags
    {
        public static string[] MOVEABLE_SURFACES = { GROUND, ONE_WAY_PLATFORM };
        public const string GROUND = "Ground";
        public const string ONE_WAY_PLATFORM = "OneWayPlatform";
    }
}
