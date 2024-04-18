using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    public static event Action levelCompleted;

    //If the player enters the level finish object trigger, they finish the level
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            levelCompleted?.Invoke();
            Time.timeScale = 0f;
        }
    }
}
