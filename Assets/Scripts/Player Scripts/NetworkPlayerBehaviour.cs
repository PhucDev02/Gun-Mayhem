using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerBehaviour : NetworkBehaviour
{
    public Actor actor;

    private float currentInputX = 0, oldInputX = 0;
    private float currentInputY = 0, oldInputY = 0;
    private bool currentFire = false, lastFire = false;
    private NetworkVariable<float> playerInputX = new NetworkVariable<float>(0f);
    private NetworkVariable<float> playerInputY = new NetworkVariable<float>(0f);
    public NetworkVariable<bool> isFireRpc = new NetworkVariable<bool>(false);

    private void Start()
    {
        //controller = GetComponent<PlayerController>();
        //Debug.LogError("IsHost: " + IsHost, gameObject);
        //Debug.LogError("IsClient: " + IsClient, gameObject);
        //Debug.LogError("IsServer: " + IsServer, gameObject);
        //Debug.LogError("IsOwner: " + IsOwner, gameObject);
        if ((IsHost && IsOwner) || (!IsHost && !IsOwner))
        {
            actor.EPlayer = EPlayer.BluePlayer;
            transform.SetPositionAndRotation(new Vector3(-5, 0, 0),
                Quaternion.Euler(0, 0, 0));
        }
        else
        {
            actor.EPlayer = EPlayer.RedPlayer;
            transform.SetPositionAndRotation(new Vector3(5, 0, 0),
                Quaternion.Euler(0, 180, 0));
        }
    }

    private void Update()
    {
        UpdateMovement();
        UpdateAttack();
        UpdateJumpAndDrop();
    }

    private void UpdateAttack()
    {
        //if(IsLocalPlayer)
        if (IsOwner && IsClient)
        {
            bool fireInput = Input.GetKey(actor.inputSetting.attack);
            if (fireInput)
            {
                //test
                //actor.action.Attack();

                currentFire = true;
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
        if (isFireRpc.Value == true)
        {
            actor.action.Attack();
        }
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
            if (Input.GetKey(actor.inputSetting.left))
            {
                currentInputX = -1;
            }
            else if (Input.GetKey(actor.inputSetting.right))
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

        if (inputX != 0)
        {
            actor.action.Move(inputX);
        }
    }

    private void UpdateJumpAndDrop()
    {
        HandleInputY();
        UpdateMovementY();
    }
    private void HandleInputY()
    {
        if (IsOwner && IsClient)
        {
            if (Input.GetKey(actor.inputSetting.jump))
            {
                currentInputY = 1;
            }
            else if (Input.GetKey(actor.inputSetting.drop))
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
            actor.action.Jump();
        else if (inputY == -1)
            actor.action.Drop();
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
}
