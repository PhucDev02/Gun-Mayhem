using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    float spawnTime;
    private void OnEnable()
    {
        spawnTime = Time.time;
        sr.color = Color.white;
    }
    void Update()
    {
        this.gameObject.transform.Translate(this.gameObject.transform.InverseTransformDirection(transform.right * GameConfig.data.bulletMoveSpeed * Time.deltaTime));
        if (Time.time - spawnTime > 2.5f)
        {
            Deactive();
        }
    }

    private void Deactive()
    {
        sr.DOFade(0, 0.5f).OnComplete(() =>
        {
            ObjectPool.Instance.Recall(this.gameObject);
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Ground" || collision.tag == "Player Bullet")
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            PlayerLives EnemyHealth = collision.GetComponent<PlayerLives>();
            if (player != null)
            {
                player.TakeDamage(GameConfig.data.bulletKnockbackForce,transform.position);
                ObjectPool.Instance.Recall(this.gameObject);
            }

            ObjectPool.Instance.Recall(this.gameObject);
        }


    }

}
