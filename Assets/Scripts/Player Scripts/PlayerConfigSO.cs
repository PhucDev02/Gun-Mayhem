using System;
using UnityEngine;

[CreateAssetMenu]
public class PlayerConfigSO : ScriptableObject
{
    public PlayerConfig[] playerConfigs;
}

[Serializable]
public class PlayerConfig
{
    public RuntimeAnimatorController animator;
    public Sprite playerIndicator;
    public Sprite playerInvincibleIndicator;
}
