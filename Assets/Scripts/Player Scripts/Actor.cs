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
    public EPlayer EPlayer
    {
        get => type;
        set
        {
            type = value;
            SetPlayer();
            life.UpdateLives();
            GameController.Instance.RegisterActor(this);
        }
    }

    public InputSetting inputSetting;
    public PoolObjectTag bulletTag;

    public Rigidbody2D rb;
    
    [Header("Jumping System")]
    [SerializeField] public OneWayPlatformHandler OneWayPlatformHandler;
    
    [Header("Animation System")]
    public Animator Animator;

    [Header("Attack System")]
    public Transform AttackPoint;

    [Header("Indicator")]
    [SerializeField] private SpriteRenderer playerIndicator;
    [SerializeField] private SpriteRenderer playerInvincibleIndicator;

    private void OnDisable()
    {
        GameController.Instance.UnRegisterActor(this);
    }

    private void OnDestroy()
    {
        GameController.Instance.UnRegisterActor(this);
    }

    public void SetPlayer()
    {
        if (type == EPlayer.BluePlayer)
        {
            bulletTag = PoolObjectTag.Bullet1;
            Animator.runtimeAnimatorController = Config.player.playerConfigs[0].animator;
            playerIndicator.sprite = Config.player.playerConfigs[0].playerIndicator;
            playerInvincibleIndicator.sprite = Config.player.playerConfigs[0].playerInvincibleIndicator;
            //Animator
        }
        else
        {
            bulletTag = PoolObjectTag.Bullet2;
            Animator.runtimeAnimatorController = Config.player.playerConfigs[1].animator;
            playerIndicator.sprite = Config.player.playerConfigs[1].playerIndicator;
            playerInvincibleIndicator.sprite = Config.player.playerConfigs[1].playerInvincibleIndicator;
        }
    }

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
