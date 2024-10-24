using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    [SerializeField] InputSetting inputSetting;
    [SerializeField] PoolObjectTag bulletTag;

    [Header("Movement System")]
    [SerializeField] private Rigidbody2D PlayerRB;
    [SerializeField] private float MoveSpeed, JumpForce;

    [Header("Jumping System")]
    [SerializeField] private GameObject GroundCheckPoint;
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private float CheckRadius;

    [Header("Animation System")]
    private Animator AnimationController;
    [SerializeField] public GameObject RankIcon;

    [Header("Attack System")]
    [SerializeField] private GameObject AttackPoint;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private AudioClip AttackSound;
    private AudioSource Audio;

    [Header("Dash System")]
    [SerializeField] private GameObject DashEffect;
    private SpriteRenderer PlayerAppearance;
}
