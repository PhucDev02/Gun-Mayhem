using System;
using Unity.Collections;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [Header("Component")]
    public ActionComponent action;
    public LifeComponent life;
    [Header("Setting")]
    public EPlayer type;
    public InputSetting inputSetting;
    public PoolObjectTag bulletTag;

    public Rigidbody2D rb;
    
    [Header("Jumping System")]
    [SerializeField] public OneWayPlatformHandler OneWayPlatformHandler;
    
    [Header("Animation System")]
    public Animator Animator;

    [Header("Attack System")]
    public Transform AttackPoint;

    public void SetVelocity(float x = float.MaxValue, float y = float.MaxValue)
    {
        if (x == float.MaxValue)
        {
            x = rb.linearVelocity.x;
        }
        if (y == float.MaxValue)
        {
            y = rb.linearVelocity.y;
        }
        rb.linearVelocity = Vector2.right * x + Vector2.up * y;
    }
    public void IncreaseLives(int amount)
    {
        life.IncreaseHp(amount);
    }

    public void BecomeInvisible(int duration)
    {
        life.ActivateGodMode(duration);
    }

    public void ChangeSpeed(int speed)
    {
        action.IncreasePlayerSpeed(speed);
    }
}
