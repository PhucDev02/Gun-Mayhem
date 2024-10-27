using UnityEngine;
[CreateAssetMenu]
public class GameConfigSO : ScriptableObject
{
    public float groundRadiusCheck = 0.25f;
    public int maxHealth = 10;
    public int moveSpeed = 4;
    public float jumpForce = 15;
    public float dashTime = 0.25f;
    public float dashCoolDownTime = 1f;
    public float attackCooldown = .5f;
    public float velocityLerpFactor = 3.0f;

    public LayerMask GroundLayerMask;
    public float recoilFactor = 1.5f;
    public float bulletMoveSpeed = 10f;
    public float bulletKnockbackForce = 5;
    public float knockbackTime=0.2f;
}
