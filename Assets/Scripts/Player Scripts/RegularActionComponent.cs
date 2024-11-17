using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class RegularActionComponent : ActionComponent
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] private GameObject GroundCheckPoint;
    private float velocity_X;

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
        CurrentAttackCoolDown = Mathf.Max(CurrentAttackCoolDown - Time.deltaTime, 0);

    }

    private bool IsOnGround()
    {
        return Physics2D.OverlapCircle(GroundCheckPoint.transform.position, Config.data.groundRadiusCheck, Config.data.GroundLayerMask);
    }
    private void UpdateMovement()
    {
        velocity_X = Mathf.Lerp(velocity_X, 0, Config.data.velocityLerpFactor * Time.deltaTime);

        actor.SetVelocity(velocity_X * MoveSpeed, float.MaxValue);
        actor.Animator.SetBool("IsGrounded", IsGrounded);
        actor.Animator.SetFloat("Horizontal Input", Mathf.Abs(velocity_X));
        actor.Animator.SetFloat("Y Velocity", actor.rb.linearVelocity.y);

    }

    public override void IncreasePlayerSpeed(float multiplier)
    {
        CancelInvoke(nameof(ReturnDefaultSpeed));
        this.MoveSpeed = multiplier * Config.data.moveSpeed;
        Invoke(nameof(ReturnDefaultSpeed), 10);
    }

    private void ReturnDefaultSpeed()
    {
        this.MoveSpeed = Config.data.moveSpeed;
    }
    public override void Move(float dir)
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

    public override void Jump()
    {
        if (IsGrounded == true)
        {
            actor.SetVelocity(float.MaxValue, Config.data.jumpForce);
        }

        if (IsGrounded == false && Abled2DoubleJump == true)
        {
            actor.SetVelocity(float.MaxValue, Config.data.jumpForce);
            Abled2DoubleJump = false;
        }
    }

    public override void Drop()
    {
        actor.OneWayPlatformHandler.OnDropMyself();
    }

    public override void Attack()
    {
        if (CurrentAttackCoolDown > 0) return;
        PresentRangeAttack();
        CurrentAttackCoolDown = Config.data.attackCooldown;
        // recoil
        velocity_X += Config.data.recoilFactor * (transform.eulerAngles.y > 90 ? 1 : -1);
    }
    public override void TakeDamage(float forceKnockback, Vector2 position)
    {
        if (actor.life.IsInvincible) return;
        CurrentKnockbackTime = Config.data.knockbackTime;

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
