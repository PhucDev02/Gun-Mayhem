using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 10f;
    [SerializeField] private SpriteRenderer sr;
    float spawnTime;
    private void OnEnable()
    {
        spawnTime = Time.time;
        sr.color = Color.white;
    }
    void Update()
    {
        this.gameObject.transform.Translate(this.gameObject.transform.InverseTransformDirection(transform.right * MoveSpeed * Time.deltaTime));
        if (Time.time - spawnTime > 2.5f)
        {
            RecallBullet();
        }
    }

    private void RecallBullet()
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
            PlayerHealth EnemyHealth = collision.GetComponent<PlayerHealth>();

            if (EnemyHealth != null)
            {
                EnemyHealth.TakeDamage(1);
                ObjectPool.Instance.Recall(this.gameObject);
            }

            ObjectPool.Instance.Recall(this.gameObject);
        }


    }

}
