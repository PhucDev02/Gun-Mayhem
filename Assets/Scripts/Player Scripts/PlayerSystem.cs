using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerHealth playerHealth;
    [SerializeField]
    private PlayerController playerController;

    public void IncreaseHp(int amount)
    {
        playerHealth.IncreaseHp(amount);
    }

    public void BecomeInvisible(int duration)
    {
        playerHealth.ActivateGodMode(duration);
    }

    public void ChangeSpeed(int speed)
    {
        playerController.IncreasePlayerSpeed(speed);
    }
}
