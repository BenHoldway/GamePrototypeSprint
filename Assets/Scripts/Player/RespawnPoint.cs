using System;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] bool isStart;

    public static event Action<Transform> SetRespawnPoint;

    private void Start()
    {
        if(isStart)
            SetRespawnPoint?.Invoke(gameObject.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isStart) 
        { 
            SetRespawnPoint?.Invoke(gameObject.transform);
        }
    }
}
