using System;
using UnityEngine;

public class ActionComponent : ActorComponent
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] private GameObject GroundCheckPoint;
   [SerializeField] private float velocity_X;

    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool Abled2DoubleJump;
    [HideInInspector] public float CurrentAttackCoolDown = 0;
    private float CurrentDashTime = 0;
    private float CurrentKnockbackTime = 0;

    void Update()
    {
        UpdateMovement();

        IsGrounded = IsOnGround();

        if (IsGrounded == true)
        {
            Abled2DoubleJump = true;
        }
        CurrentKnockbackTime = Mathf.Max(CurrentKnockbackTime - Time.deltaTime, 0);
        CurrentDashTime = Mathf.Max(CurrentDashTime - Time.deltaTime, 0);
        CurrentAttackCoolDown = Mathf.Max(CurrentAttackCoolDown - Time.deltaTime, 0);

    }

    private bool IsOnGround()
    {
        return Physics2D.OverlapCircle(GroundCheckPoint.transform.position, GameConfig.data.groundRadiusCheck, GameConfig.data.GroundLayerMask);
    }
    private void UpdateMovement()
    {
        velocity_X = Mathf.Lerp(velocity_X, 0, GameConfig.data.velocityLerpFactor * Time.deltaTime);

        actor.SetVelocity(velocity_X * MoveSpeed, float.MaxValue);
        actor.Animator.SetBool("IsGrounded", IsGrounded);
        actor.Animator.SetFloat("Horizontal Input", Mathf.Abs(velocity_X));
        actor.Animator.SetFloat("Y Velocity", actor.rb.linearVelocity.y);

    }


    public void IncreasePlayerSpeed(float multiplier)
    {
        CancelInvoke(nameof(ReturnDefaultSpeed));
        this.MoveSpeed = multiplier * GameConfig.data.moveSpeed;
        Invoke(nameof(ReturnDefaultSpeed), 10);
    }

    private void ReturnDefaultSpeed()
    {
        this.MoveSpeed = GameConfig.data.moveSpeed;
    }
    public void Move(float dir)
    {
        if (CurrentKnockbackTime > 0) return;
        if (dir < 0)
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (dir > 0)
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (Mathf.Abs(velocity_X + dir) > Mathf.Abs(dir) * 2) return;
        else
            velocity_X += dir;
    }

    public void Jump()
    {
        if (IsGrounded == true && CurrentDashTime <= 0)
        {
            actor.SetVelocity(float.MaxValue, GameConfig.data.jumpForce);
        }

        if (IsGrounded == false && Abled2DoubleJump == true && CurrentDashTime <= 0)
        {
            actor.SetVelocity(float.MaxValue, GameConfig.data.jumpForce);
            Abled2DoubleJump = false;
        }
    }

    public void Drop()
    {
        actor.OneWayPlatformHandler.OnDropMyself();
    }

    public void Attack()
    {
        if (CurrentAttackCoolDown > 0) return;
        PresentRangeAttack();
        CurrentAttackCoolDown = GameConfig.data.attackCooldown;
        // recoil
        velocity_X += GameConfig.data.recoilFactor * (transform.eulerAngles.y > 90 ? 1 : -1);
    }

    public void TakeDamage(float forceKnockback, Vector2 position)
    {
        if (actor.life.IsInvincible) return;
        CurrentKnockbackTime = GameConfig.data.knockbackTime;

        Vector2 norm = (Vector2)transform.position - position;
        norm.Normalize();
        norm.y = 0;
        velocity_X += forceKnockback * (norm.x > 0 ? 1 : -1);
        actor.SetVelocity(float.MaxValue, actor.rb.linearVelocityY + norm.y * forceKnockback);
    }


    public void PresentRangeAttack()
    {
        actor.Animator.SetTrigger("Attack");
        //Audio.PlayOneShot(AttackSound);
        //attack sound
        var bullet = ObjectPool.Instance.Spawn(actor.bulletTag);
        bullet.GetComponent<PlayerBullet>().Setup(actor.type, actor.AttackPoint);
        //Recoil();
    }
}
