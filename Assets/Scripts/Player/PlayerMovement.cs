using Cinemachine;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(ManageInput))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    AreaChecks areaChecks;
    PlayerGrapple grapple;
    PlayerJump jump;
    ManageInput input;

    [SerializeField] float speed;
    bool isFacingRight;
    bool isMoving;
    Vector3 scale;
    Vector2 movement;

    Vector2 roomChangeVelocity;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        areaChecks = GetComponent<AreaChecks>();
        grapple = GetComponent<PlayerGrapple>();
        jump = GetComponent<PlayerJump>();

        input = GetComponent<ManageInput>();

        isFacingRight = true;
        scale = transform.localScale;
    }

    private void OnEnable()
    {
        //Reads the value when the move input is started
        input.PlayerControls.Player.Move.started += ctx => 
        {
            movement = ctx.ReadValue<Vector2>();
        };
        //Reads the value when the move input is cancelled - stops the player from continuing to move
        input.PlayerControls.Player.Move.canceled += ctx =>
        {
            movement = ctx.ReadValue<Vector2>();
            rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
        };

        //Will 'squish' the player so they crouch
        input.PlayerControls.Player.Crouch.started += ctx =>
        {
            scale.y *= 0.5f;
            transform.localScale = scale;
            Vector2 pos = transform.localPosition;
            pos.y -= 0.25f;
            transform.localPosition = pos;
        };

        //Will 'unsquish' the player so they stand back up
        input.PlayerControls.Player.Crouch.canceled += ctx =>
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

    private void OnDisable()
    {
        AreaManager.ChangeRoom -= DisableInput;
        CameraManager.RoomSuccessfullyChanged -= EnableInput;
    }

    // Update is called once per frame
    void Update()
    {
        //Will move the player when the input value is above 0.02f in the direction of travel
        if(Mathf.Abs(movement.x) > 0.02f)
            rb.velocity = new Vector2 (movement.x * speed, rb.velocity.y);

        //If the movement is opposite to the direction that the player is facing, then call the flip function
        if ((isFacingRight && movement.x < 0) || (!isFacingRight && movement.x > 0))
            Flip();

        if (movement.x == 0f && !(rb.velocity.x == 0f) && !grapple.HasGrappled && rb.velocity.y == 0f)
            if(areaChecks.IsGrounded())
                rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void Flip() 
    { 
        //Will swap the direction that the player is facing
        isFacingRight = !isFacingRight;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    public void DisableInput(bool _isFirstRoom) 
    {
        roomChangeVelocity = rb.velocity;

        //Disables input
        input.enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
            
        //Gets 2 units in front of where the player is facing, and sets the player's position to this new position
        if(!_isFirstRoom)
        {
            Vector2 newPos = new Vector2(transform.position.x + (scale.x * 2f), transform.position.y);
            transform.position = newPos;
        }

        jump.enabled = false;
        grapple.enabled = false;
    }

    public void EnableInput() 
    {
        //Disables input
        input.enabled = true;
        rb.isKinematic = false;

        rb.AddForce(roomChangeVelocity, ForceMode2D.Impulse);

        grapple.enabled = true;
        
        jump.enabled = true;
        grapple.enabled = true;
    }
}
