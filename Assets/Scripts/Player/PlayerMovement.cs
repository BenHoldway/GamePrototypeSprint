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
    [SerializeField] float crouchSpeed;

    float currentSpeed;
    bool isFacingRight;
    bool isUncrouching;
    Vector3 scale;
    public Vector3 Scale { get { return scale; } private set { value = scale; } }
    Vector2 movement;

    public Vector2 roomChangeVelocity { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        areaChecks = GetComponent<AreaChecks>();
        grapple = GetComponent<PlayerGrapple>();
        jump = GetComponent<PlayerJump>();

        input = GetComponent<InputManager>();

        isFacingRight = true;
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

            currentSpeed = crouchSpeed;
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
        //Will move the player when the input value is above 0.02f in the direction of travel
        if(Mathf.Abs(movement.x) > 0.02f)
            rb.velocity = new Vector2 (movement.x * currentSpeed, rb.velocity.y);

        //If the movement is opposite to the direction that the player is facing, then call the flip function
        if ((isFacingRight && movement.x < 0) || (!isFacingRight && movement.x > 0))
            Flip();

        //Debug.Log(movement.x == 0f && rb.velocity.x >= 0.1f && !grapple.HasGrappled && rb.velocity.y <= 0.1f);

        if (jump.IsWallJumping)
            currentSpeed = wallJumpSpeed;
        else
            currentSpeed = normalSpeed;

        
        if (movement.x == 0f && rb.velocity.x >= 0.1f && !grapple.HasGrappled && rb.velocity.y <= 0.1f)
        {
            Debug.Log("AA");
            if(areaChecks.IsGrounded())
            {
                Debug.Log("AAAAAAAAAAA");
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

                currentSpeed = normalSpeed;
                isUncrouching = false;
            }
            Debug.Log(areaChecks.CanUncrouch());
        }

    }

    public void Flip() 
    { 
        //Will swap the direction that the player is facing
        isFacingRight = !isFacingRight;
        scale.x *= -1f;
        transform.localScale = scale;
    }


    public void EnableInput() 
    {
        //Enables input
        input.enabled = true;
        rb.isKinematic = false;

        rb.velocity = Vector2.zero;
        Debug.Log(roomChangeVelocity);
        rb.AddForce(roomChangeVelocity, ForceMode2D.Impulse);

        grapple.enabled = true;
        
        jump.enabled = true;
        grapple.enabled = true;
    }
}
