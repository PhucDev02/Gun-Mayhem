using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] EPlayer from;
    [SerializeField] private SpriteRenderer sr;
    float spawnTime;
    public void Setup(EPlayer from,Transform fromTransform)
    {
        this.from = from;
        transform.SetLocalPositionAndRotation(fromTransform.position, fromTransform.rotation);
    }
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
            if (from == EPlayer.AI)
            {
                Messenger.Broadcast(EventKey.OnMissTarget);
            }
            ObjectPool.Instance.Recall(this.gameObject);
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Ground" || collision.tag == "Player Bullet")
        {
            Actor actor = collision.GetComponent<Actor>();
            if (actor != null)
            {
                ObjectPool.Instance.Spawn(PoolObjectTag.HitText, UIEffectCanvas.Instance.transform).transform.position = collision.ClosestPoint(actor.transform.position);
                actor.action.TakeDamage(GameConfig.data.bulletKnockbackForce, transform.position);
                ObjectPool.Instance.Recall(this.gameObject);
            }
            else
            ObjectPool.Instance.Recall(this.gameObject);
        }


    }

}
