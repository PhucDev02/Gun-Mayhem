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
            PlayerLives EnemyHealth = collision.GetComponent<PlayerLives>();

            if (EnemyHealth != null)
            {
                ObjectPool.Instance.Recall(this.gameObject);
            }

            ObjectPool.Instance.Recall(this.gameObject);
        }


    }

}
