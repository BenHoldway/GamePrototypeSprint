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

    [SerializeField] int maxJumpNum;
    int currentJump;

    [SerializeField] float jumpingPower;

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
            if(IsGrounded())
                Jump();

            if (ctx.canceled && rb.velocity.y > 0f)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        };
    }


    bool onGround;
    // Update is called once per frame
    void Update()
    {
        //if (IsOnWall() && rb.velocity.y < 0f)
        //    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

        onGround = Physics2D.BoxCast(groundCheck.position, new Vector2(transform.localScale.x, 0.1f), 0, Vector2.down, groundLayer);
    }


    void Grapple()
    {

    }

    void Jump()
    {
        currentJump++;
        //Debug.Log("Jumped");

        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
    }

    bool IsGrounded()
    {
        if (currentJump < maxJumpNum)
        {
         //   print(currentJump);
            return true;
        }

        bool isOnGround = Physics2D.BoxCast(new Vector2(0, -1), new Vector2(transform.localScale.x, 0.05f), 0, Vector2.down);

        Debug.Log(isOnGround);

        if(isOnGround)
        {
            currentJump = 0;
        }

        return isOnGround;
    }

    //bool IsOnWall() 
    //{
    //    if (!IsGrounded())
    //        if (!Physics2D.BoxCast(rightSideCheck.position, new Vector2(0.05f, transform.localScale.y), 0, transform.right, groundLayer))
    //            return Physics2D.BoxCast(leftSideCheck.position, new Vector2(0.05f, transform.localScale.y), 0, -transform.right, groundLayer);

    //    return false;
    //}










    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position - transform.up * 0.05f, new Vector3(transform.localScale.x, 0.1f, 0.01f));
       // Gizmos.DrawWireCube(leftSideCheck.position - transform.right * 0.025f, new Vector3(0.05f, transform.localScale.y, 0.01f));
       // Gizmos.DrawWireCube(rightSideCheck.position + transform.right * 0.025f, new Vector3(0.05f, transform.localScale.y, 0.01f));
    }
}
