using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BulletLogic : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private GameObject LockedTarget;
    [SerializeField] private Rigidbody2D BulletRB;
    private int target;
    private GameObject[] Targets;

    private Vector2 direction;
   
    public void ArrestPlayer(GameObject player)
    {
        if (player)
        {
            LockedTarget = player;
            direction = (player.transform.position - transform.position).normalized;
        }
        else
        {
            ReleaseBullet();
        }

        Invoke(nameof(ReleaseBullet), 4f);
    }
    void Update()
    {
        if (LockedTarget != null)
        {
            BulletRB.linearVelocity = direction * MoveSpeed;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerHealth TargetHealth = collision.GetComponent<PlayerHealth>();
            if (TargetHealth != null) 
            {
                TargetHealth.TakeDamage(1);
            }
            ReleaseBullet();
        }
    }

    private void ReleaseBullet()
    {
        CancelInvoke(nameof(ReleaseBullet));
        ObjectPool.Instance.Recall(gameObject);
    }
}
