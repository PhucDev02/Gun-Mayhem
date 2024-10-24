using UnityEngine;

public interface IPlayerAction
{
    public void Move(float dir);
    public void Jump();
    public void RangedAttack();
    public void MeleeAttack();
    public void TakeDamage(int damage);
    public void Dash();
    public void IncreasePlayerSpeed(float speed);
}
