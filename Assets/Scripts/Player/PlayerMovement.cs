using Cinemachine;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float speed;
    bool isFacingRight;
    bool isMoving;
    Vector3 scale;
    Vector2 movement;

    PlayerControls playerControls;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isFacingRight = true;
        scale = transform.localScale;

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        //Reads the value when the move input is started
        playerControls.Player.Move.started += ctx => 
        {
            movement = ctx.ReadValue<Vector2>();
        };
        //Reads the value when the move input is cancelled - stops the player from continuing to move
        playerControls.Player.Move.canceled += ctx =>
        {
            movement = ctx.ReadValue<Vector2>();
        };

        //Will 'squish' the player so they crouch
        playerControls.Player.Crouch.started += ctx =>
        {
            scale.y *= 0.5f;
            transform.localScale = scale;
            Vector2 pos = transform.localPosition;
            pos.y -= 0.25f;
            transform.localPosition = pos;
        };
        //Will 'unsquish' the player so they stand back up
        playerControls.Player.Crouch.canceled += ctx =>
        {
            scale.y *= 2.0f;
            transform.localScale = scale;
            Vector2 pos = transform.localPosition;
            pos.y += 0.25f;
            transform.localPosition = pos;
        };

        AreaManager.ChangeRoom += DisableInput;
        CameraManager.RoomSuccessfullyChanged += EnableInput;
    }

    // Update is called once per frame
    void Update()
    {
        //Will move the player when the input value is above 0.02f in the direction of travel
        if(movement.x > 0.02f || movement.x < 0.02f)
            rb.velocity = new Vector2 (movement.x * speed, rb.velocity.y);

        //If the movement is opposite to the direction that the player is facing, then call the flip function
        if ((isFacingRight && movement.x < 0) || (!isFacingRight && movement.x > 0))
            Flip();
    }

    void Flip() 
    { 
        //Will swap the direction that the player is facing
        isFacingRight = !isFacingRight;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    void DisableInput() 
    {
        //Disables input
        playerControls.Disable();
        
        //Gets 2 units in front of where the player is facing, and sets the player's position to this new position
        Vector2 newPos = new Vector2(transform.position.x + (scale.x * 2f), transform.position.y);
        transform.position = newPos;
    }

    void EnableInput() 
    {
        //Disables input
        playerControls.Enable();
    }
}
