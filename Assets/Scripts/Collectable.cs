using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public static event Action collectableCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collectableCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
