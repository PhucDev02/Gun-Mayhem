using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AIBehavior : Agent
{
    [SerializeField] Actor actor;
    Vector3 initialPosition;
  
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(actor.action.IsGrounded);
        sensor.AddObservation(actor.action.Abled2DoubleJump);
        sensor.AddObservation(actor.action.CurrentAttackCoolDown);
        sensor.AddObservation(GameController.Instance.GetTargetPosition());
        sensor.AddObservation((Vector2)transform.position);
    }
    public override void OnEpisodeBegin()
    {
        this.transform.position = initialPosition + Vector3.up * 1;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var discreteActions = actions.DiscreteActions;
        if (discreteActions[0] == 1)
        {
            actor.action.Move(1);
            AddReward(-0.05f);
        }
        else if (discreteActions[0] == 2)
        {
            actor.action.Move(-1);
            AddReward(-0.05f);
        }

        if (discreteActions[1] == 1)
        {
            actor.action.Attack();
        }
        if (discreteActions[2] == 1)
        {
            actor.action.Jump();
        }
        if (discreteActions[3] == 1)
        {
            actor.action.Drop();
        }


    }
    public override void Initialize()
    {
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions.Clear();
        if (Input.GetKey(actor.inputSetting.right))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(actor.inputSetting.left))
        {
            discreteActions[0] = 2;
        }
        if (Input.GetKeyDown(actor.inputSetting.attack))
        {
            discreteActions[1] = 1;
        }
        if (Input.GetKeyDown(actor.inputSetting.jump))
        {
            discreteActions[2] = 1;
        }
        if (Input.GetKeyDown(actor.inputSetting.drop))
        {
            discreteActions[3] = 1;
        }
    }
    private void Start()
    {
        initialPosition = this.transform.position;
        AddListener();
    }
    private void OnDestroy()
    {
        RemoveListener();
    }

    private void RemoveListener()
    {
        Messenger.RemoveListener(EventKey.OnHitDummy, OnHitTarget);
        Messenger.RemoveListener(EventKey.OnMissTarget, OnMissTarget);
        Messenger.RemoveListener(EventKey.OnDie, OnDie);
    }

    private void AddListener()
    {
        Messenger.AddListener(EventKey.OnHitDummy, OnHitTarget);
        Messenger.AddListener(EventKey.OnMissTarget, OnMissTarget);
        Messenger.AddListener(EventKey.OnDie, OnDie);
    }

    private void OnDie()
    {
        AddReward(-1);
    }

    private void OnMissTarget()
    {
        AddReward(-0.1f);
    }

    private void OnHitTarget()
    {
        AddReward(1);
    }

    
}