using System.Collections;
using UnityEngine;
using Cinemachine;
using System;

public class AreaManager : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] bool isFirstRoom;

    Vector2 newAreaDirection;

    public static event Action<bool, Vector2> ChangeRoom;

    private void OnEnable()
    {
        SetNewAreaDirection.SetDirection += SetDirection;
    }

    private void SetDirection(Vector2 _direction)
    {
        newAreaDirection = _direction;
    }

    //Will enable room virtual camera when player enters the area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            cam.SetActive(true);
            ChangeRoom?.Invoke(isFirstRoom, newAreaDirection);
        }
    }

    //Hides the virtual camera when player leaves the area, so that only one virtual camera will be active at once
    //(Only active virtual camera will be in new room)
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            cam.SetActive(false);
        }
    }
}
