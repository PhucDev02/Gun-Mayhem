using UnityEngine;

[CreateAssetMenu(fileName = "BoosterData", menuName = "ScriptableObjects/BoosterData", order = 1)]
public class BoostersData : ScriptableObject
{
    [SerializeField]
    private Booster[] boosters;

    public Booster[] Boosters { get => boosters; }

    public Booster GetBooster(BoosterEffectType boosterEffectType) 
        => boosters[(int)boosterEffectType];

}
