using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ActionComponent : ActorComponent
{

    [ReadOnly] public bool IsGrounded;
    [ReadOnly] public bool Abled2DoubleJump;
    [ReadOnly] public float CurrentAttackCoolDown = 0;
    public abstract void Jump();
    public abstract void Move(float dir);
    public abstract void Drop();
    public abstract void Attack();
    public abstract void TakeDamage(float force,Vector2 impactPoint);
    public abstract void IncreasePlayerSpeed(float speed);
}
