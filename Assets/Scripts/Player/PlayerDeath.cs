using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerDeath : MonoBehaviour
{
    Rigidbody2D rb;
    Transform respawnPoint;

    public static event Action death;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        RespawnPoint.SetRespawnPoint += SetRespawnPoint;
    }

    void SetRespawnPoint(Transform _respawnPoint)
    {
        respawnPoint = _respawnPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle") 
        { 
            if(respawnPoint != null)
            {
                transform.position = respawnPoint.position;
                rb.velocity = Vector2.zero;
                death?.Invoke();
            }
        }
    }
}
