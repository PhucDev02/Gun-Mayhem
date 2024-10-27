using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { 
            PlayerLives PlayerHP = collision.GetComponent<PlayerLives>();
        }
    }
}
