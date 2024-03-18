using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class PlayerJump : MonoBehaviour
{
    PlayerControls playerControls;
    Rigidbody2D rb;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

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

            if(ctx.canceled && rb.velocity.y > 0f)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Grapple()
    {

    }

    void Dash()
    {

    }

    void Jump()
    {
        currentJump++;
        Debug.Log("Jumped");

        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
    }

    bool IsGrounded()
    {
        if(currentJump < maxJumpNum)
            return true;

        bool isOnGround = Physics2D.OverlapCircle(groundCheck.position, 0.01f, groundLayer);

        if(isOnGround)
            currentJump = 0;

        return isOnGround;
    }
}
