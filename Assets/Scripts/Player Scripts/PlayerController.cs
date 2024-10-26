
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerHealth health;
    [SerializeField]
    private IPlayerAction action;
    public PlayerReference reference;
    private void Start()
    {
        action=GetComponent<IPlayerAction>();
    }
    public void IncreaseHp(int amount)
    {
        health.IncreaseHp(amount);
    }

    public void BecomeInvisible(int duration)
    {
        health.ActivateGodMode(duration);
    }

    public void ChangeSpeed(int speed)
    {
        action.IncreasePlayerSpeed(speed);
    }

    internal void TakeDamage(float force, Vector3 position)
    {
        //health.TakeDamage(force);
        action.TakeDamage(force, position);
    }
}
