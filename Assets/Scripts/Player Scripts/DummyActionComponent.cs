using UnityEngine;

public class DummyActionComponent : ActionComponent
{
    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void Drop()
    {
        throw new System.NotImplementedException();
    }

    public override void IncreasePlayerSpeed(float speed)
    {
    }

    public override void Jump()
    {
        throw new System.NotImplementedException();
    }

    public override void Move(float dir)
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage(float force, Vector2 impactPoint)
    {
        actor.Animator.SetTrigger("TakeDamage");
        Messenger.Broadcast(EventKey.OnHitDummy);
    }
}
