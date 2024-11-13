using Unity.Netcode;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

public class PlayerAction : NetworkBehaviour, IPlayerAction
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

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        //Debug.LogError("IsHost: " + IsHost, gameObject);
        //Debug.LogError("IsClient: " + IsClient, gameObject);
        //Debug.LogError("IsServer: " + IsServer, gameObject);
        //Debug.LogError("IsOwner: " + IsOwner, gameObject);
        if((IsHost && IsOwner) || (!IsHost && !IsOwner))
        {
            controller.EPlayer = EPlayer.BluePlayer;
            transform.position = new Vector3(-5, 0, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            controller.EPlayer = EPlayer.RedPlayer;
            transform.position = new Vector3(5, 0, 0);
            transform.rotation = Quaternion.Euler(0, 180, 0);
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

        if(CurrentKnockbackTime > 0) CurrentKnockbackTime -= Time.deltaTime;
        if(CurrentDashTime > 0) CurrentDashTime -= Time.deltaTime;
        if(CurrentDashCoolDown > 0) CurrentDashCoolDown -= Time.deltaTime;
        if(CurrentAttackCoolDown > 0) CurrentAttackCoolDown -= Time.deltaTime;
    }

    private Vector2 currentInput = Vector2.zero;
    private Vector2 oldInput = Vector2.zero;
    private bool lastFire = false, currentFire = false; 
    public NetworkVariable<Vector2> playerMovementRpc = new NetworkVariable<Vector2>();
    public NetworkVariable<bool> isFireRpc = new NetworkVariable<bool>();


    private void UpdateMovement()
    {
        velocity_X = Mathf.Lerp(velocity_X, 0, Config.data.velocityLerpFactor * Time.deltaTime);

        if (IsOwner && IsClient)
        {
            if (Input.GetKey(controller.reference.inputSetting.left))
            {
                currentInput.x = -1;
                if (currentInput.x != oldInput.x)
                {
                    oldInput.x = currentInput.x;
                    SynchMovementServerRpc(new Vector2(-1, 100));
                }
            }
            else if (Input.GetKey(controller.reference.inputSetting.right))
            {
                currentInput.x = 1;
                if (currentInput.x != oldInput.x)
                {
                    oldInput.x = currentInput.x;
                    SynchMovementServerRpc(new Vector2(1, 100));
                }
            }
            else
            {
                currentInput.x = 0;
                if (currentInput.x != oldInput.x)
                {
                    oldInput.x = currentInput.x;
                    SynchMovementServerRpc(new Vector2(0, 100));
                }
            }

        }
        
        if (playerMovementRpc.Value.x == -1)
        {
            Move(-1);
            this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (playerMovementRpc.Value.x == 1)
        {
            Move(1);
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        // Movement
        controller.reference.Animator.SetBool("IsGrounded", IsGrounded);
        controller.reference.Animator.SetFloat("Horizontal Input", Mathf.Abs(velocity_X));
        controller.reference.Animator.SetFloat("Y Velocity", controller.reference.Rb.linearVelocity.y);
        controller.reference.SetVelocity(velocity_X * MoveSpeed, float.MaxValue);
    }

    [ServerRpc]
    public void SynchMovementServerRpc(Vector2 moveVector)
    {
        if (moveVector.x == 100) playerMovementRpc.Value = new Vector2(playerMovementRpc.Value.x, moveVector.y);
        else if (moveVector.y == 100) playerMovementRpc.Value = new Vector2(moveVector.x, playerMovementRpc.Value.y);
        else playerMovementRpc.Value = moveVector;
    }

    [ServerRpc]
    public void SyncFireBulletServerRpc(bool isFire)
    {
        this.isFireRpc.Value = isFire;
    }

    private void UpdateJumpUpAndDown()
    {
        if (IsOwner && IsClient)
        {
            if (Input.GetKey(controller.reference.inputSetting.jump))
            {
                currentInput.y = 1;
                if (currentInput.y != oldInput.y)
                {
                    oldInput.y = currentInput.y;
                    SynchMovementServerRpc((new Vector2(100, 1)));
                }
            }
            else if (Input.GetKey(controller.reference.inputSetting.drop))
            {
                currentInput.y = -1;
                if (currentInput.y != oldInput.y)
                {
                    oldInput.y = currentInput.y;
                    SynchMovementServerRpc((new Vector2(100, -1)));
                }
            }
            else
            {
                currentInput.y = 0;
                if (currentInput.y != oldInput.y)
                {
                    oldInput.y = currentInput.y;
                    SynchMovementServerRpc((new Vector2(100, 0)));
                }
            }
        }
        if (playerMovementRpc.Value.y == 1)
            Jump();
        else if (playerMovementRpc.Value.y == -1)
            Drop();
    }

    private void UpdateAttack()
    {
        if (IsOwner && IsClient)
        {
            bool fireInput = Input.GetKey(controller.reference.inputSetting.attack);
            //if (Input.GetKeyDown(controller.reference.inputSetting.attack) && CurrentAttackCoolDown <= 0)
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
        Debug.Log("Call jump");
        if (IsGrounded == true && CurrentDashTime <= 0)
        {
            controller.reference.SetVelocity(float.MaxValue, Config.data.jumpForce);
            Debug.Log("Jump 1");
        }

        if (IsGrounded == false && Abled2DoubleJump == true && CurrentDashTime <= 0)
        {
            controller.reference.SetVelocity(float.MaxValue, Config.data.jumpForce);
            Abled2DoubleJump = false;
            Debug.Log("Jump 2");
        }
    }

    public void Drop()
    {
        controller.reference.OneWayPlatformHandler.OnDropMyself();
    }

    public void RangedAttack()
    {
        Debug.Log("Shooted");
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
        CurrentDashTime = Config.data.dashTime;
        CurrentDashCoolDown = Config.data.dashCoolDownTime;
    }

    public void MeleeAttack()
    {

    }
}
//both player and AI have the same action,animation and movement -> should use abstract
//but can not use for AI because AI must inherit from  AIAgent, and normal player inherit from MonoBehaviour