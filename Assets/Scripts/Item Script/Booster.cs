using UnityEngine;

public enum BoosterEffectType
{
    IncreaseHP,
    IncreaseSpeed,
    SwapWeapon,
    BecomeInvisible,
}

[System.Serializable]
public class Booster
{
    public BoosterEffectType effectType;
    public Sprite boosterSpr;
    public int value;
    public float rate;

    public Booster(BoosterEffectType effectType, int value, float rate)
    {
        this.effectType = effectType;
        this.value = value;
        this.rate = rate;
    }
}