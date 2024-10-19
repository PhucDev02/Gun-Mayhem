using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance;
    public BoostersData boosterData;

    public Booster[] BoosterArr => boosterData.Boosters;

    private void Awake()
    {
        Instance = this;
    }

    public Booster GetRandomBoosterByRate()
    {
        float totalRate = 0;
        foreach (Booster booster in BoosterArr)
        {
            totalRate += booster.rate;
        }

        float randomValue = Random.Range(0, totalRate);
        float cumulativeRate = 0;

        foreach (Booster booster in BoosterArr)
        {
            cumulativeRate += booster.rate;
            if (randomValue <= cumulativeRate)
            {
                return booster;
            }
        }

        return null;
    }

    public void ActivateBooster(Booster booster, PlayerHealth playerHealth)
    {
        switch (booster.effectType)
        {
            case BoosterEffectType.IncreaseHP:
                playerHealth.HealMode();
                Debug.Log("Increased HP by: " + booster.value);
                break;

            case BoosterEffectType.IncreaseSpeed:
                //Player.Instance.AddSpeed(booster.value);
                Debug.Log("Increased Speed by: " + booster.value);
                break;

            case BoosterEffectType.SwapWeapon:
                //Player.Instance.ChangeWeapon();
                Debug.Log("Swapped Weapon");
                break;

            case BoosterEffectType.BecomeInvisible:
                playerHealth.ActiveGodMode();
                Debug.Log("Became Invisible for: " + booster.value + " seconds");
                break;

        }
    }
}
