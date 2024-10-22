using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { 
            PlayerHealth PlayerHP = collision.GetComponent<PlayerHealth>();
            if (PlayerHP)
            {
                PlayerHP.TakeDamage(1);
            }
        }
    }
}
