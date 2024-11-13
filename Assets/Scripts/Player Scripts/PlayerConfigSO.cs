using System;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu]
public class PlayerConfigSO : ScriptableObject
{
    public PlayerConfig[] playerConfigs;
}

[Serializable]
public class PlayerConfig
{
    public AnimatorController animator;
    public Sprite playerIndicator;
    public Sprite playerInvincibleIndicator;
}
