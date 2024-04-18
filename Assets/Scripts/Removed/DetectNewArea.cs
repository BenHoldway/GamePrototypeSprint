using System;
using UnityEngine;

public class DetectNewArea : MonoBehaviour
{
    public static event Action<GameObject> GoToNewArea;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If player enters new area trigger collider, it will load
        if (collision.gameObject.tag == "AreaBounds") 
        { 
            GoToNewArea?.Invoke(collision.gameObject);
        }
    }
}
