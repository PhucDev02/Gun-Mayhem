using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AIAction : Agent, IPlayerAction
{
    Vector3 initialPosition;
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
        sensor.AddObservation(IsGrounded);
        sensor.AddObservation(Abled2DoubleJump);
        sensor.AddObservation(CurrentAttackCoolDown);
        sensor.AddObservation(GameController.Instance.GetTargetPosition());
    }
    public override void OnEpisodeBegin()
    {
        this.transform.position = initialPosition + Vector3.up * 1;
        IsGrounded = false;
        Abled2DoubleJump = false;
        velocity_X = 0;
        ReturnDefaultSpeed();
        CurrentAttackCoolDown = 0;
        CurrentDashTime = 0;
        CurrentDashCoolDown = 0;
        CurrentKnockbackTime = 0;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var discreteActions = actions.DiscreteActions;
        if (discreteActions[0] == 1)
        {
            Move(1);
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (discreteActions[0] == 2)
        {
            Move(-1);
            this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (discreteActions[1] == 1)
        {
            RangedAttack();
        }
        if (discreteActions[2] == 1)
        {
            Jump();
        }
        if (discreteActions[3] == 1)
        {
            Drop();
        }


    }
    private void Update()
    {
        //update movement
        velocity_X = Mathf.Lerp(velocity_X, 0, GameConfig.data.velocityLerpFactor * Time.deltaTime);

        controller.reference.SetVelocity(velocity_X * MoveSpeed, float.MaxValue);
        controller.reference.Animator.SetBool("IsGrounded", IsGrounded);
        controller.reference.Animator.SetFloat("Horizontal Input", Mathf.Abs(velocity_X));
        controller.reference.Animator.SetFloat("Y Velocity", controller.reference.Rb.linearVelocity.y);
        IsGrounded = controller.reference.IsOnGround();
        //
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
    public override void Initialize()
    {
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions.Clear();
        if (Input.GetKey(controller.reference.inputSetting.right))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(controller.reference.inputSetting.left))
        {
            discreteActions[0] = 2;
        }
        if (Input.GetKey(controller.reference.inputSetting.attack))
        {
            discreteActions[1] = 1;
        }
        if (Input.GetKey(controller.reference.inputSetting.jump))
        {
            discreteActions[2] = 1;
        }
        if (Input.GetKey(controller.reference.inputSetting.drop))
        {
            discreteActions[3] = 1;
        }
    }
    private void Start()
    {
        controller = GetComponent<PlayerController>();
        initialPosition = this.transform.position;
        AddListener();
    }
    private void OnDestroy()
    {
        RemoveListener();
    }

    private void RemoveListener()
    {
        Messenger.RemoveListener(EventKey.OnHitTarget, OnHitTarget);
        Messenger.RemoveListener(EventKey.OnMissTarget, OnMissTarget);
        Messenger.RemoveListener(EventKey.OnDie, OnDie);
    }

    private void AddListener()
    {
        Messenger.AddListener(EventKey.OnHitTarget, OnHitTarget);
        Messenger.AddListener(EventKey.OnMissTarget, OnMissTarget);
        Messenger.AddListener(EventKey.OnDie, OnDie);
    }

    private void OnDie()
    {
        AddReward(-1);
    }

    private void OnMissTarget()
    {
        AddReward(-0.5f);
    }

    private void OnHitTarget()
    {
        AddReward(1);
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
        if (IsGrounded == true && CurrentDashTime <= 0)
        {
            controller.reference.SetVelocity(float.MaxValue, GameConfig.data.jumpForce);
        }

        if (IsGrounded == false && Abled2DoubleJump == true && CurrentDashTime <= 0)
        {
            controller.reference.SetVelocity(float.MaxValue, GameConfig.data.jumpForce);
            Abled2DoubleJump = false;
        }
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
        if (CurrentKnockbackTime > 0) return;
        if (Mathf.Abs(velocity_X + dir) > Mathf.Abs(dir) * 2) return;
        else
            velocity_X += dir;
    }

    public void RangedAttack()
    {
        if (CurrentAttackCoolDown <= 0)
        {
            controller.reference.PresentRangeAttack();
            CurrentAttackCoolDown = GameConfig.data.attackCooldown;
            // recoil
            velocity_X += GameConfig.data.recoilFactor * (transform.eulerAngles.y > 90 ? 1 : -1);
        }
    }

    public void TakeDamage(float forceKnockback, Vector2 position)
    {
        if (controller.playerLives.IsInvincible) return;
        CurrentKnockbackTime = GameConfig.data.knockbackTime;

        Vector2 norm = (Vector2)transform.position - position;
        norm.Normalize();
        norm.y = 0;
        velocity_X += forceKnockback * (norm.x > 0 ? 1 : -1);
        controller.reference.SetVelocity(float.MaxValue, controller.reference.Rb.linearVelocityY + norm.y * forceKnockback);
    }
    
}