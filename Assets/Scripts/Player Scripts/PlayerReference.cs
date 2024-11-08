using System;
using Unity.Collections;
using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    public InputSetting inputSetting;
    [SerializeField] PoolObjectTag bulletTag;

    [Header("Movement System")]
    public Rigidbody2D Rb;

    [Header("Jumping System")]
    [SerializeField] private GameObject GroundCheckPoint;
    
    [Header("Jumping System")]
    [SerializeField] public OneWayPlatformHandler OneWayPlatformHandler;
    
    [Header("Animation System")]
    public Animator Animator;

    [Header("Attack System")]
    [SerializeField] private GameObject AttackPoint;
    [SerializeField] private AudioClip AttackSound;
    [SerializeField] private AudioSource Audio;

    [Header("Dash System")]
    [SerializeField] private GameObject DashEffect;
    [SerializeField] private SpriteRenderer Sr;
    void OnValidate()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = this.gameObject.GetComponent<Animator>();
        Sr = this.gameObject.GetComponent<SpriteRenderer>();
        Audio = this.gameObject.GetComponent<AudioSource>();
    }
    public bool IsOnGround()
    {
        return Physics2D.OverlapCircle(GroundCheckPoint.transform.position, GameConfig.data.groundRadiusCheck, GameConfig.data.GroundLayerMask);
    }
    public void PresentRangeAttack()
    {
        Animator.SetTrigger("Attack");
        Audio.PlayOneShot(AttackSound);
        var bullet = ObjectPool.Instance.Spawn(bulletTag);
        bullet.transform.SetLocalPositionAndRotation(AttackPoint.transform.position, AttackPoint.transform.rotation);
        //Recoil();
    }
    public void SetVelocity(float x = float.MaxValue, float y = float.MaxValue)
    {
        if (x == float.MaxValue)
        {
            x = Rb.linearVelocity.x;
        }
        if (y == float.MaxValue)
        {
            y = Rb.linearVelocity.y;
        }
        Rb.linearVelocity = Vector2.right * x + Vector2.up * y;
    }

    internal void PresentDashShadow()
    {
        GameObject Effect = Instantiate(DashEffect, this.gameObject.transform.position, this.gameObject.transform.rotation);
        SpriteRenderer Shadow = Effect.GetComponent<SpriteRenderer>();
        Shadow.sprite = Sr.sprite;
        Shadow.color = new Color(1, 1, 1, 0.5f);
    }
    public void Recoil()
    {
        Vector3 recoilDirection = Vector2.left * (transform.rotation.y < 90 ? 1 : -1) * GameConfig.data.recoilFactor;
        Debug.Log(recoilDirection.x);
        Rb.linearVelocity = new Vector2(recoilDirection.x, Rb.linearVelocityY);
    }
}
