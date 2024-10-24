using UnityEngine;

public class PlayerAction : MonoBehaviour, IPlayerAction
{
    private PlayerController controller;
    [SerializeField] InputSetting inputSetting;
    [SerializeField] PoolObjectTag bulletTag;

    [Header("Movement System")]
    [SerializeField] private Rigidbody2D PlayerRB;
    [SerializeField] private float MoveSpeed;
    private float PlayerVelocity_X;

    [Header("Jumping System")]
    [SerializeField] private GameObject GroundCheckPoint;
    [SerializeField] private LayerMask WhatIsGround;
    private bool IsGrounded;
    private bool Abled2DoubleJump;

    [Header("Animation System")]
    private Animator AnimationController;
    [SerializeField] public GameObject RankIcon;

    [Header("Attack System")]
    [SerializeField] private GameObject AttackPoint;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private AudioClip AttackSound;
    private float CurrentAttackCoolDown = 0;
    private AudioSource Audio;

    [Header("Dash System")]
    [SerializeField] private GameObject DashEffect;
    private SpriteRenderer PlayerAppearance;
    private float CurrentDashTime = 0;
    private float CurrentDashCoolDown = 0;
    private void Start()
    {
        controller = GetComponent<PlayerController>();
    }
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

        IsGrounded = Physics2D.OverlapCircle(GroundCheckPoint.transform.position, ConstValue.groundRadiusCheck, WhatIsGround);

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
            Debug.Log(1);
            PlayerRB.linearVelocity = new Vector2(PlayerVelocity_X * 3 * MoveSpeed, 0);
            GameObject Effect = Instantiate(DashEffect, this.gameObject.transform.position, this.gameObject.transform.rotation);
            SpriteRenderer Shadow = Effect.GetComponent<SpriteRenderer>();
            Shadow.sprite = PlayerAppearance.sprite;
            Shadow.color = new Color(1, 1, 1, 0.5f);
        }

    }

    private void UpdateMovement()
    {
        PlayerVelocity_X = 0;
        if (Input.GetKey(inputSetting.left))
        {
            Move(-1);
            this.gameObject.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        if (Input.GetKey(inputSetting.right))
        {
            Move(1);
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
            Dash();
        }
    }

    private void UpdateJump()
    {
        if (!Input.GetKeyDown(inputSetting.jump)) return;
        Jump();
    }

    private void UpdateAttack()
    {
        if (Input.GetKey(inputSetting.attack) && CurrentAttackCoolDown <= 0)
        {
            RangedAttack();
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
        this.MoveSpeed = ConstValue.moveSpeed;
    }
    public void Move(float dir)
    {
        PlayerVelocity_X = dir;
    }

    public void Jump()
    {
        if (IsGrounded == true && CurrentDashTime <= 0)
        {
            PlayerRB.linearVelocity = new Vector2(PlayerRB.linearVelocity.x, ConstValue.jumpForce);
        }

        if (IsGrounded == false && Abled2DoubleJump == true && CurrentDashTime <= 0)
        {
            PlayerRB.linearVelocity = new Vector2(PlayerRB.linearVelocity.x, ConstValue.jumpForce);
            Abled2DoubleJump = false;
        }
    }

    public void RangedAttack()
    {
        AnimationController.SetTrigger("Attack");
        Audio.PlayOneShot(AttackSound);
        var bullet = ObjectPool.Instance.Spawn(bulletTag);
        bullet.transform.SetLocalPositionAndRotation(AttackPoint.transform.position, AttackPoint.transform.rotation);
        CurrentAttackCoolDown = ConstValue.attackCooldown;
    }

    public void TakeDamage(int damage)
    {
    }

    public void Dash()
    {
        CurrentDashTime = ConstValue.dashTime;
        CurrentDashCoolDown = ConstValue.dashCoolDownTime;
    }

    public void MeleeAttack()
    {
        throw new System.NotImplementedException();
    }
}
//both player and AI have the same action,animation and movement -> should use abstract
//but can not use for AI because AI must inherit from  AIAgent, and normal player inherit from MonoBehaviour