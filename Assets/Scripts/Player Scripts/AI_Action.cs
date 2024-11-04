using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AI_Action : Agent, IPlayerAction
{
    private PlayerController controller;

    [SerializeField] private float MoveSpeed;
    private float velocity_X;

    private bool IsGrounded;
    private bool Abled2DoubleJump;

    private float CurrentAttackCoolDown = 0;
    private float CurrentDashTime = 0;
    private float CurrentDashCoolDown = 0;
    private float CurrentKnockbackTime = 0;
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
    }
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
    }
    private void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        UpdateMovement();
        UpdateAttack();
        UpdateJump();
        UpdateDrop();
        UpdateDash();

        IsGrounded = controller.reference.IsOnGround();

        if (IsGrounded == true)
        {
            Abled2DoubleJump = true;
        }
        CurrentKnockbackTime -= Time.deltaTime;
        CurrentKnockbackTime = Mathf.Max(CurrentKnockbackTime, 0);

        CurrentDashTime -= Time.deltaTime;
        CurrentDashTime = Mathf.Max(CurrentDashTime, 0);

        CurrentDashCoolDown -= Time.deltaTime;
        CurrentDashCoolDown = Mathf.Max(CurrentDashCoolDown, 0);

        CurrentAttackCoolDown -= Time.deltaTime;
        CurrentAttackCoolDown = Mathf.Max(CurrentAttackCoolDown, 0);
        //dash
        if (CurrentDashTime > 0 && Mathf.Abs(velocity_X) > 0)
        {
            controller.reference.SetVelocity(velocity_X * 3 * MoveSpeed, 0);
            controller.reference.PresentDashShadow();
        }

    }

    private void UpdateMovement()
    {
        velocity_X = Mathf.Lerp(velocity_X, 0, GameConfig.data.velocityLerpFactor * Time.deltaTime);
        if (Input.GetKey(controller.reference.inputSetting.left))
        {
            Move(-1);
            this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetKey(controller.reference.inputSetting.right))
        {
            Move(1);
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Movement

        controller.reference.SetVelocity(velocity_X * MoveSpeed, float.MaxValue);
        controller.reference.Animator.SetBool("IsGrounded", IsGrounded);
        controller.reference.Animator.SetFloat("Horizontal Input", Mathf.Abs(velocity_X));
        controller.reference.Animator.SetFloat("Y Velocity", controller.reference.Rb.linearVelocity.y);

    }
    private void UpdateDash()
    {
        if (Input.GetKeyDown(controller.reference.inputSetting.dash) && CurrentDashCoolDown <= 0)
        {
            Dash();
        }
    }

    private void UpdateJump()
    {
        if (!Input.GetKeyDown(controller.reference.inputSetting.jump)) return;
        Jump();
    }

    private void UpdateDrop()
    {
        if (!Input.GetKeyDown(controller.reference.inputSetting.drop)) return;
        Drop();
    }

    private void UpdateAttack()
    {
        if (Input.GetKey(controller.reference.inputSetting.attack) && CurrentAttackCoolDown <= 0)
        {
            RangedAttack();
        }
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
    public void Dash()
    {
    }


    public void Jump()
    {
    }

    public void Drop()
    {
        controller.reference.OneWayPlatformHandler.OnDropMyself();
    }
    public void MeleeAttack()
    {
    }

    public void Move(float dir)
    {
    }

    public void RangedAttack()
    {
    }

    public void TakeDamage(float forceKnockback, Vector2 position)
    {
    }
}