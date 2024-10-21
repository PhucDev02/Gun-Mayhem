using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireGolem : MonoBehaviour
{
    private Animator AnimationController;
    private float ShootCountDown;
    private bool Attack;
    [SerializeField] private GameObject Bullet;

    private GameObject[] targets;


    // Start is called before the first frame update
    void Start()
    {
        ShootCountDown = 3f;
        AnimationController = this.gameObject.GetComponent<Animator>();

        targets = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        ShootCountDown -= Time.deltaTime;

        if (ShootCountDown < 0)
        {
            Attack = true;
            ShootCountDown = 7.5f;
            SpawnFireBullet();
        } else 
        {
            Attack = false; 
        }
        AnimationController.SetBool("Attack", Attack);
    }

    private void SpawnFireBullet()
    {

        for(int  i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                var bullet = ObjectPool.Instance.Spawn(PoolObject.TowerBullet);
                bullet.transform.position = transform.position;
                bullet.GetComponent<BulletLogic>().ArrestPlayer(targets[i]);
            }
        }
    }
}
