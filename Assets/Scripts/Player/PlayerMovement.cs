using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    PlayerControls controls;
    Rigidbody2D rb;

    [SerializeField] float speed;
    InputAction playerMove;
    Vector2 movement;

    // Start is called before the first frame update
    void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        controls?.Enable();

        playerMove = controls.Player.Move;
    }

    private void OnDisable()
    {
        controls?.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        movement = playerMove.ReadValue<Vector2>().normalized;
        if(movement.magnitude > 0.1f)
            rb.velocity = movement * speed;
    }
}
