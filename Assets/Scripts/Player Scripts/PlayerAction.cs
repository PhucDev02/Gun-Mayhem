using UnityEngine;

public class PlayerAction : MonoBehaviour, IPlayerAction
{
    private PlayerController controller;
    
    [SerializeField] private float MoveSpeed;
    private float velocity_X;

    private bool IsGrounded;
    private bool Abled2DoubleJump;

    private float CurrentAttackCoolDown = 0;
    private float CurrentDashTime = 0;
    private float CurrentDashCoolDown = 0;
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
        velocity_X = 0;
        if (Input.GetKey(controller.reference.inputSetting.left))
        {
            Move(-1);
            this.gameObject.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        if (Input.GetKey(controller.reference.inputSetting.right))
        {
            Move(1);
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Movement

        controller.reference.SetVelocity(velocity_X * MoveSpeed,float.MaxValue);
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
        this.MoveSpeed = multiplier*ConstValue.moveSpeed;
        Invoke(nameof(ReturnDefaultSpeed), 10);
    }

    private void ReturnDefaultSpeed()
    {
        this.MoveSpeed = ConstValue.moveSpeed;
    }
    public void Move(float dir)
    {
        velocity_X = dir;
    }

    public void Jump()
    {
        if (IsGrounded == true && CurrentDashTime <= 0)
        {
            controller.reference.SetVelocity(float.MaxValue, ConstValue.jumpForce);
        }

        if (IsGrounded == false && Abled2DoubleJump == true && CurrentDashTime <= 0)
        {
            controller.reference.SetVelocity(float.MaxValue, ConstValue.jumpForce);
            Abled2DoubleJump = false;
        }
    }

    public void Drop()
    {
        controller.reference.OneWayPlatformHandler.OnDropMyself();
    }

    public void RangedAttack()
    {
        controller.reference.PresentRangeAttack();
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