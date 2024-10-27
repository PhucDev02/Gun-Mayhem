
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private EPlayer ePlayer;
    [SerializeField]
    public PlayerLives playerLives;
    [SerializeField]
    private IPlayerAction action;
    public PlayerReference reference;

    public EPlayer EPlayer { get => ePlayer; set => ePlayer = value; }

    private void Start()
    {
        action=GetComponent<IPlayerAction>();
    }

    public void Register(PlayerReference playerReference)
    {
        this.reference = playerReference;
    }

    public void Register(PlayerLives playerLives)
    {
        this.playerLives = playerLives;
    }

    public void IncreaseLives(int amount)
    {
        playerLives.IncreaseHp(amount);
    }

    public void BecomeInvisible(int duration)
    {
        playerLives.ActivateGodMode(duration);
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
