using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private GameObject TeleportTargetPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.transform.position = TeleportTargetPoint.transform.position +
                                                      TeleportTargetPoint.transform.TransformDirection(new Vector3(1,0,0));
        }
    }
}
