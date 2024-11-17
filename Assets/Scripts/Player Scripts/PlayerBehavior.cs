
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
        if (Input.GetKeyDown(actor.inputSetting.attack))
        {
            actor.action.Attack();
        }
        //Jump
        if(Input.GetKeyDown(actor.inputSetting.jump))
        {
            actor.action.Jump();
        }
        //drop
        if(Input.GetKeyDown(actor.inputSetting.drop))
        {
            actor.action.Drop();
        }
    }

}
