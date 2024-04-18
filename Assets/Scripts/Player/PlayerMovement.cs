using Cinemachine;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(InputManager))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    AreaChecks areaChecks;
    PlayerGrapple grapple;
    PlayerJump jump;
    InputManager input;

    [SerializeField] float normalSpeed;
    [SerializeField] float wallJumpSpeed;

    float currentSpeed;
    public bool IsFacingRight { get; private set; }
    bool isUncrouching;
    Vector3 scale;
    public Vector3 Scale { get { return scale; } private set { value = scale; } }
    Vector2 movement;
    float movementDir;

    public Vector2 roomChangeVelocity { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        areaChecks = GetComponent<AreaChecks>();
        grapple = GetComponent<PlayerGrapple>();
        jump = GetComponent<PlayerJump>();

        input = GetComponent<InputManager>();

        IsFacingRight = true;
        scale = transform.localScale;

        currentSpeed = normalSpeed;
    }

    private void OnEnable()
    {
        //Reads the value when the move input is started
        input.PlayerControls.Player.Move.performed += ctx => 
        {
            movement = ctx.ReadValue<Vector2>();
        };
        //Reads the value when the move input is cancelled - stops the player from continuing to move
        input.PlayerControls.Player.Move.canceled += ctx =>
        {
            movement = ctx.ReadValue<Vector2>();
            rb.velocity = new Vector2(movement.x * currentSpeed, rb.velocity.y);
        };

        //Will 'squish' the player so they crouch
        input.PlayerControls.Player.Crouch.started += ctx =>
        {
            if (isUncrouching)
                return;

            scale.y *= 0.5f;
            transform.localScale = scale;
            Vector2 pos = transform.localPosition;
            pos.y -= 0.25f;
            transform.localPosition = pos;
        };

        //Will 'unsquish' the player so they stand back up
        input.PlayerControls.Player.Crouch.canceled += ctx =>
        {
            isUncrouching = true;
        };

        CameraManager.RoomSuccessfullyChanged += EnableInput;
    }

    private void OnDisable()
    {
        CameraManager.RoomSuccessfullyChanged -= EnableInput;

        movement = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (jump.IsWallJumping)
        {
            currentSpeed = wallJumpSpeed;
            movementDir = jump.WallJumpDir;
        }
        else
        {
            currentSpeed = normalSpeed;
            movementDir = movement.x;
        }

        //Will move the player when the input value is above 0.02f in the direction of travel
        if (Mathf.Abs(movementDir) > 0.02f)
            rb.velocity = new Vector2 (movementDir * currentSpeed, rb.velocity.y);

        //If the movement is opposite to the direction that the player is facing, then call the flip function
        if ((IsFacingRight && movementDir < 0) || (!IsFacingRight && movement.x > 0) && !jump.IsWallJumping)
            Flip();


        if (movementDir == 0f && Mathf.Abs(rb.velocity.x) >= 0.1f && !grapple.IsGrappling && rb.velocity.y <= 0.1f)
        {
            if (areaChecks.IsGrounded())
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }


        if(isUncrouching)
        {
            if(areaChecks.CanUncrouch())
            {
                scale.y *= 2.0f;
                transform.localScale = scale;
                Vector2 pos = transform.localPosition;
                pos.y += 0.25f;
                transform.localPosition = pos;
                isUncrouching = false;
            }
        }

    }

    public void Flip() 
    { 
        //Will swap the direction that the player is facing
        IsFacingRight = !IsFacingRight;
        scale.x *= -1f;
        transform.localScale = scale;
    }


    public void EnableInput() 
    {
        //Enables input
        input.enabled = true;
        rb.isKinematic = false;

        rb.velocity = Vector2.zero;
        rb.AddForce(roomChangeVelocity, ForceMode2D.Impulse);
        
        jump.enabled = true;
        grapple.enabled = true;
    }
}
