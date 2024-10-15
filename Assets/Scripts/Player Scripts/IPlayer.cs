using UnityEngine;

public interface IPlayer
{
    public void Move(float dir);
    public void Jump();
    public void Attack(int damage);
    public void TakeDamage(int damage);

}
