using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] InputSetting inputSetting;
    [SerializeField] PoolObjectTag bulletTag;
    public const int MOVE_SPEED = 4;

    [Header("Movement System")]
    [SerializeField] private Rigidbody2D PlayerRB;
    [SerializeField] private float MoveSpeed, JumpForce;
    private float PlayerVelocity_X;

    [Header("Jumping System")]
    [SerializeField] private GameObject GroundCheckPoint;
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private float CheckRadius;
    private bool IsGrounded;
    private bool Abled2DoubleJump;

    [Header("Animation System")]
    private Animator AnimationController;
    [SerializeField] public GameObject RankIcon;

    [Header("Attack System")]
    [SerializeField] private GameObject AttackPoint;
    [SerializeField] private float AttackCoolDown;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private AudioClip AttackSound;
    private float CurrentAttackCoolDown = 0;
    private AudioSource Audio;

    [Header("Dash System")]
    [SerializeField] private float DashTime;
    [SerializeField] private float DashCoolDown;
    [SerializeField] private GameObject DashEffect;
    private SpriteRenderer PlayerAppearance;
    private float CurrentDashTime = 0;
    private float CurrentDashCoolDown = 0;

    void OnValidate()
    {
        AnimationController = this.gameObject.GetComponent<Animator>();
        PlayerAppearance = this.gameObject.GetComponent<SpriteRenderer>();
        Audio = this.gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        UpdateMovement();
        UpdateAttack();
        UpdateJump();
        UpdateDash();
        RankIcon.transform.rotation = Quaternion.Euler(0, 0, 0);

        IsGrounded = Physics2D.OverlapCircle(GroundCheckPoint.transform.position, CheckRadius, WhatIsGround);

        if (IsGrounded == true)
        {
            Abled2DoubleJump = true;
        }

        CurrentDashTime -= Time.deltaTime;

        if (CurrentDashTime < 0)
        {
            CurrentDashTime = 0;
        }

        CurrentDashCoolDown -= Time.deltaTime;

        if (CurrentDashCoolDown < 0)
        {
            CurrentDashCoolDown = 0;
        }

        CurrentAttackCoolDown -= Time.deltaTime;

        if (CurrentAttackCoolDown < 0)
        {
            CurrentAttackCoolDown = 0;
        }
        //dash
        if (CurrentDashTime > 0 && Mathf.Abs(PlayerVelocity_X) > 0)
        {
            PlayerRB.linearVelocity = new Vector2(PlayerVelocity_X * 3 * MoveSpeed, 0);
            GameObject Effect = Instantiate(DashEffect, this.gameObject.transform.position, this.gameObject.transform.rotation);
            SpriteRenderer Shadow = Effect.GetComponent<SpriteRenderer>();
            Shadow.sprite = PlayerAppearance.sprite;
            Shadow.color = new Color(1, 1, 1, 0.05f);
        }

    }

    private void UpdateMovement()
    {
        PlayerVelocity_X = 0;
        if (Input.GetKey(inputSetting.left))
        {
            PlayerVelocity_X = -1;
            this.gameObject.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        if (Input.GetKey(inputSetting.right))
        {
            PlayerVelocity_X = 1;
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Movement
        PlayerRB.linearVelocity = new Vector2(PlayerVelocity_X * MoveSpeed, PlayerRB.linearVelocity.y);

        AnimationController.SetBool("IsGrounded", IsGrounded);
        AnimationController.SetFloat("Horizontal Input", Mathf.Abs(PlayerVelocity_X));
        AnimationController.SetFloat("Y Velocity", PlayerRB.linearVelocity.y);

    }
    private void UpdateDash()
    {
        if (Input.GetKeyDown(inputSetting.dash) && CurrentDashCoolDown <= 0)
        {
            CurrentDashTime = DashTime;
            CurrentDashCoolDown = DashCoolDown;
        }
    }

    private void UpdateJump()
    {
        if (!Input.GetKeyDown(inputSetting.jump)) return;
        if (IsGrounded == true && CurrentDashTime <= 0)
        {
            PlayerRB.linearVelocity = new Vector2(PlayerRB.linearVelocity.x, JumpForce);
        }

        if (IsGrounded == false && Abled2DoubleJump == true && CurrentDashTime <= 0)
        {
            PlayerRB.linearVelocity = new Vector2(PlayerRB.linearVelocity.x, JumpForce);
            Abled2DoubleJump = false;
        }
    }

    private void UpdateAttack()
    {
        if (Input.GetKey(inputSetting.attack) && CurrentAttackCoolDown <= 0)
        {
            AnimationController.SetTrigger("Attack");
            Audio.PlayOneShot(AttackSound);
            var bullet= ObjectPool.Instance.Spawn(bulletTag);
            bullet.transform.SetLocalPositionAndRotation(AttackPoint.transform.position, AttackPoint.transform.rotation);
            //Instantiate(Bullet, AttackPoint.transform.position, AttackPoint.transform.rotation);
            CurrentAttackCoolDown = AttackCoolDown;
        }
    }

    public void IncreasePlayerSpeed(float speed)
    {
        CancelInvoke(nameof(ReturnDefaultSpeed));
        this.MoveSpeed = speed;
        Invoke(nameof(ReturnDefaultSpeed), 10);
    }

    private void ReturnDefaultSpeed()
    {
        this.MoveSpeed = MOVE_SPEED;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GroundCheckPoint.transform.position, CheckRadius);
    }

    public void Move(float dir)
    {
        throw new System.NotImplementedException();
    }

    public void Jump()
    {
        throw new System.NotImplementedException();
    }

    public void Attack(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
}