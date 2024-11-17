using Unity.Netcode;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

public class Network_Action : NetworkBehaviour, IPlayerAction
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

    //networking data store
    private float currentInputX = 0, oldInputX = 0;
    private float currentInputY = 0, oldInputY = 0;
    private bool currentFire = false, lastFire = false;
    private NetworkVariable<float> playerInputX = new NetworkVariable<float>(0f);
    private NetworkVariable<float> playerInputY = new NetworkVariable<float>(0f);
    public NetworkVariable<bool> isFireRpc = new NetworkVariable<bool>(false);

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        //Debug.LogError("IsHost: " + IsHost, gameObject);
        //Debug.LogError("IsClient: " + IsClient, gameObject);
        //Debug.LogError("IsServer: " + IsServer, gameObject);
        //Debug.LogError("IsOwner: " + IsOwner, gameObject);
        if ((IsHost && IsOwner) || (!IsHost && !IsOwner))
        {
            controller.EPlayer = EPlayer.BluePlayer;
            transform.SetPositionAndRotation(new Vector3(-5, 0, 0), 
                Quaternion.Euler(0, 0, 0));
        }
        else
        {
            controller.EPlayer = EPlayer.RedPlayer;
            transform.SetPositionAndRotation(new Vector3(5, 0, 0), 
                Quaternion.Euler(0, 180, 0));
        }
    }

    void Update()
    {
        UpdateMovement();
        UpdateJumpUpAndDown();
        UpdateAttack();

        IsGrounded = controller.reference.IsOnGround();

        if (IsGrounded == true)
        {
            Abled2DoubleJump = true;
        }

        if (CurrentKnockbackTime > 0) CurrentKnockbackTime -= Time.deltaTime;
        if (CurrentDashTime > 0) CurrentDashTime -= Time.deltaTime;
        if (CurrentDashCoolDown > 0) CurrentDashCoolDown -= Time.deltaTime;
        if (CurrentAttackCoolDown > 0) CurrentAttackCoolDown -= Time.deltaTime;
    }


    private void UpdateMovement()
    {
        HandleInputX();
        UpdateMovementX();
    }

    private void HandleInputX()
    {
        if (IsOwner && IsClient)
        {
            if (Input.GetKey(controller.reference.inputSetting.left))
            {
                currentInputX = -1;
            }
            else if (Input.GetKey(controller.reference.inputSetting.right))
            {
                currentInputX = 1;
            }
            else
            {
                currentInputX = 0;

            }

            if (currentInputX != oldInputX)
            {
                oldInputX = currentInputX;
                SendMovementInputXServerRpc(currentInputX);
            }

        }
    }

    private void UpdateMovementX()
    {
        float inputX = playerInputX.Value;
        velocity_X = Mathf.Lerp(velocity_X, 0, Config.data.velocityLerpFactor * Time.deltaTime);
        
        if (inputX != 0)
        {
            Move(inputX);
            transform.rotation = Quaternion.Euler(0, inputX < 0 ? 180 : 0, 0);
        }

        // Movement
        controller.reference.Animator.SetBool("IsGrounded", IsGrounded);
        controller.reference.Animator.SetFloat("Horizontal Input", Mathf.Abs(velocity_X));
        controller.reference.Animator.SetFloat("Y Velocity", controller.reference.Rb.linearVelocity.y);
        controller.reference.SetVelocity(velocity_X * MoveSpeed, float.MaxValue);
    }


    private void UpdateJumpUpAndDown()
    {
        HandleInputY();
        UpdateMovementY();
    }
    private void HandleInputY()
    {
        if (IsOwner && IsClient)
        {
            if (Input.GetKey(controller.reference.inputSetting.jump))
            {
                currentInputY = 1;
            }
            else if (Input.GetKey(controller.reference.inputSetting.drop))
            {
                currentInputY = -1;
            }
            else
            {
                currentInputY = 0;
            }

            if (currentInputY != oldInputY)
            {
                oldInputY = currentInputY;
                SendMovementInputYServerRpc(currentInputY);
            }
        }
    }
    private void UpdateMovementY()
    {
        float inputY = playerInputY.Value;
        if (inputY == 1)
            Jump();
        else if (inputY == -1)
            Drop();
    }

    private void UpdateAttack()
    {
        if (IsOwner && IsClient)
        {
            bool fireInput = Input.GetKey(controller.reference.inputSetting.attack);
            if (fireInput)
            {
                currentFire = true;
                //RangedAttack();
                if (lastFire != currentFire)
                {
                    lastFire = currentFire;
                    SyncFireBulletServerRpc(true);
                }
            }
            else
            {
                currentFire = false;
                if (lastFire != currentFire)
                {
                    lastFire = currentFire;
                    SyncFireBulletServerRpc(false);
                }
            }
        }

        if (isFireRpc.Value == true && CurrentAttackCoolDown <= 0)
        {
            RangedAttack();
        }
    }
    [ServerRpc]
    public void SendMovementInputXServerRpc(float inputX)
    {
        playerInputX.Value = inputX;
    }

    [ServerRpc]
    public void SendMovementInputYServerRpc(float inputY)
    {
        playerInputY.Value = inputY;
    }

    [ServerRpc]
    public void SyncFireBulletServerRpc(bool isFire)
    {
        this.isFireRpc.Value = isFire;
    }

    public void IncreasePlayerSpeed(float multiplier)
    {
        CancelInvoke(nameof(ReturnDefaultSpeed));
        this.MoveSpeed = multiplier * Config.data.moveSpeed;
        Invoke(nameof(ReturnDefaultSpeed), 10);
    }

    private void ReturnDefaultSpeed()
    {
        this.MoveSpeed = Config.data.moveSpeed;
    }
    public void Move(float dir)
    {
        if (CurrentKnockbackTime > 0) return;
        if (Mathf.Abs(velocity_X + dir) > Mathf.Abs(dir) * 2) return;
        else
            velocity_X += dir;
    }

    public void Jump()
    {
        if (IsGrounded == true && CurrentDashTime <= 0)
        {
            controller.reference.SetVelocity(float.MaxValue, Config.data.jumpForce);
        }

        if (IsGrounded == false && Abled2DoubleJump == true && CurrentDashTime <= 0)
        {
            controller.reference.SetVelocity(float.MaxValue, Config.data.jumpForce);
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
        CurrentAttackCoolDown = Config.data.attackCooldown;
        // recoil
        velocity_X += Config.data.recoilFactor * (transform.eulerAngles.y > 90 ? 1 : -1);
    }

    public void TakeDamage(float forceKnockback, Vector2 position)
    {
        if (controller.playerLives.IsInvincible) return;
        CurrentKnockbackTime = Config.data.knockbackTime;

        Vector2 norm = (Vector2)transform.position - position;
        norm.Normalize();
        norm.y = 0;
        velocity_X += forceKnockback * (norm.x > 0 ? 1 : -1);
        controller.reference.SetVelocity(float.MaxValue, controller.reference.Rb.linearVelocityY + norm.y * forceKnockback);
    }

    public void Dash()
    {
        //CurrentDashTime = Config.data.dashTime;
        //CurrentDashCoolDown = Config.data.dashCoolDownTime;
    }

    public void MeleeAttack()
    {

    }
}
