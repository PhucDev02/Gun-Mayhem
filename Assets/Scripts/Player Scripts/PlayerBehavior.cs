
using System;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public Actor actor;
    private void Update()
    {
        //move
        if (Input.GetKey(actor.inputSetting.left))
        {
            actor.action.Move(-1);
        }
        if (Input.GetKey(actor.inputSetting.right))
        {
            actor.action.Move(1);
        }
        //attack
        if (Input.GetKey(actor.inputSetting.attack))
        {
            actor.action.Attack();
        }
        //Jump
        if(Input.GetKey(actor.inputSetting.jump))
        {
            actor.action.Jump();
        }
        //drop
        if(Input.GetKey(actor.inputSetting.drop))
        {
            actor.action.Drop();
        }
    }
    public void IncreaseLives(int amount)
    {
        actor.life.IncreaseHp(amount);
    }

    public void BecomeInvisible(int duration)
    {
        actor.life.ActivateGodMode(duration);
    }

    public void ChangeSpeed(int speed)
    {
        actor.action.IncreasePlayerSpeed(speed);
    }

    internal void TakeDamage(float force, Vector3 position)
    {
        //health.TakeDamage(force);
        actor.action.TakeDamage(force, position);
    }
}
