using UnityEngine;

public interface IPlayer
{
    public void Move(float dir);
    public void Jump();
    public void Attack();
    public void TakeDamage(int damage);
    public void Dash();
}
