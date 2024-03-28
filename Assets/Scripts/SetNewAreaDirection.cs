using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNewAreaDirection : MonoBehaviour
{
    //[Range(0f, 1f)]
    [SerializeField] Vector2 direction;
    public static event Action<Vector2> SetDirection;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log(gameObject.name);
            SetDirection?.Invoke(direction);
        }
    }
}
