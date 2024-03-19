using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerJump : MonoBehaviour
{
    PlayerControls playerControls;
    Rigidbody2D rb;

    [SerializeField] Transform groundCheck;
    [SerializeField] Transform leftSideCheck;
    [SerializeField] Transform rightSideCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    bool isOnGround;
    bool isJumping;

    [SerializeField] int maxJumpNum;
    int currentJump;

    [SerializeField] float jumpingPower;
    [SerializeField] float normalGravityScale;
    [SerializeField] float maxFallSpeed;
    [SerializeField] float fallMultiplier;

    float coyoteTime = 0.2f;
    float coyoteTimeCounter;

    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        currentJump = 0;
    }

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.Player.ActionKey.performed += ctx =>
        {
            if(currentJump < maxJumpNum && coyoteTimeCounter > 0f)
            {
                isJumping = true;
                Jump();
            }
        };

        playerControls.Player.ActionKey.canceled += ctx =>
        {
            isJumping = false;
            if (rb.velocity.y > 0f)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        };
    }


    bool onGround;
    // Update is called once per frame
    void Update()
    {
        isOnGround = IsGrounded();

        if (IsOnWall() && rb.velocity.y < 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

        GravityControl();

        if (isOnGround && !isJumping)
        {
            currentJump = 0;
            coyoteTimeCounter = coyoteTime;
        }
        else if (currentJump == 0)
            coyoteTimeCounter -= Time.deltaTime;
    }


    void Grapple()
    {

    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        currentJump++;
    }

    void GravityControl() 
    { 
        if(rb.velocity.y > 0f) 
        { 
            rb.gravityScale = normalGravityScale * fallMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else if(rb.gravityScale != normalGravityScale)
            rb.gravityScale = normalGravityScale;
    }

    bool IsGrounded()
    {
        if(Physics2D.OverlapBox(groundCheck.position, new Vector2(transform.localScale.x, 0.1f), 0, groundLayer))
        {
            return true;
        }

        return false;
    }

    bool IsOnWall() 
    {
        if (!isOnGround)
        {
            bool isOnWall = Physics2D.OverlapBox(rightSideCheck.position, new Vector2(transform.localScale.x, 0.1f), 0, wallLayer);
            if (!isOnWall)
                isOnWall = Physics2D.OverlapBox(leftSideCheck.position, new Vector2(transform.localScale.x, 0.1f), 0, wallLayer);

            return isOnWall;
        }
        return false;
    }










    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(transform.localScale.x, 0.1f));
       // Gizmos.DrawWireCube(leftSideCheck.position - transform.right * 0.025f, new Vector3(0.05f, transform.localScale.y, 0.01f));
       // Gizmos.DrawWireCube(rightSideCheck.position + transform.right * 0.025f, new Vector3(0.05f, transform.localScale.y, 0.01f));
    }
}
