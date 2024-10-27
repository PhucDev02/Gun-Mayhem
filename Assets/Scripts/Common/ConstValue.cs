using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class ConstValue
{
    
    public const float groundRadiusCheck= 0.25f;
    public const int maxLives = 3;
    public const int moveSpeed = 4;
    public const float jumpForce = 15;
    public const float dashTime = 0.25f;
    public const float dashCoolDownTime = 1f;
    public const float attackCooldown=.5f;
    public static Vector2 environmentLimitX = new Vector2(-10, 10);


    public class Tags
    {
        public static string[] MOVEABLE_SURFACES = { GROUND, ONE_WAY_PLATFORM };
        public const string GROUND = "Ground";
        public const string ONE_WAY_PLATFORM = "OneWayPlatform";
    }
}
