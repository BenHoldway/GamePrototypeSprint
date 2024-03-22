using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerMovement playerMovement;
    Rigidbody2D rb;

    #region Jumping
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
 
    bool isOnGround;
    bool isJumping;

    [SerializeField] int maxJumpNum;
    int currentJump;

    [SerializeField] float jumpingPower;
    [SerializeField] float normalGravityScale;
    [SerializeField] float maxFallSpeed;
    [SerializeField] float fallMultiplier;

    float coyoteMaxTime = 0.2f;
    float coyoteTimer;
    #endregion

    #region WallSliding
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask wallLayer;

    bool isWallSliding;

        #region WallJumping
        [SerializeField] Vector2 wallJumpPower;
        [SerializeField] float wallJumpMaxTime;
        [SerializeField] float wallJumpDuration;
        bool isWallJumping;
        float wallJumpDir;
        float wallJumpTimer;
        #endregion
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        currentJump = 0;
    }

    private void OnEnable()
    {
        playerControls.Enable();

        //Jump is performed
        playerControls.Player.ActionKey.performed += ctx =>
        {
            WallJump();

            //Runs if the timer is above 0 and able to wall jump
            if(wallJumpTimer > 0f)
            {
                //Flips the player if the player is not facing the direction of the wall jump
                if(transform.localScale.x != wallJumpDir)
                    playerMovement.Flip();

                isWallJumping = true;
                rb.velocity = new Vector2(wallJumpDir * wallJumpPower.x, wallJumpPower.y);
                wallJumpTimer = 0;
                currentJump++;

                //Stops the wall jump
                Invoke(nameof(StopWallJump), wallJumpDuration);
            }
            //Makes the player jump if the player isn't wall jumping and can jump normally
            else if(currentJump < maxJumpNum && coyoteTimer > 0f)
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

        //If player is on wall, not on the ground and falling, set vertical speed to sliding speed
        if (IsOnWall() && !isOnGround && rb.velocity.y < 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        //Disables the player wall sliding
        else
            isWallSliding = false;

        //Handles the gravity depending on if the player is falling or not
        GravityControl();

        //Resets the jumping counter and coyote time
        if (isOnGround && !isJumping)
        {
            currentJump = 0;
            coyoteTimer = coyoteMaxTime;
        }
        //Decreases coyote time when player isn't on the ground
        else if (currentJump == 0)
            coyoteTimer -= Time.deltaTime;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        currentJump++;
    }

    //Handles the wall jump
    private void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = true;
            //Gets the direction of the player, and inverses it
            wallJumpDir = -transform.localScale.x;
            wallJumpTimer = wallJumpMaxTime;
            CancelInvoke(nameof(StopWallJump));
        }
        //Decreases wall jump time
        else
            wallJumpTimer -= Time.deltaTime;
    }

    //Stops the player wall jump
    void StopWallJump()
    {
        isWallJumping = false;
    }

    //Controls gravity amount
    void GravityControl() 
    { 
        //If player is falling, gravity increases over time until it reaches the max fall speed
        if(rb.velocity.y < 0f) 
        { 
            rb.gravityScale = normalGravityScale * fallMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        //Sets gravity back to normal if player is not falling
        else if(rb.gravityScale != normalGravityScale)
            rb.gravityScale = normalGravityScale;
    }

    //Checks if player is on the ground
    bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, new Vector2(transform.localScale.x, 0.1f), 0, groundLayer);
    }

    //Checks if player is up against the wall
    bool IsOnWall() 
    {
        return Physics2D.OverlapBox(wallCheck.position, new Vector2(0.1f, transform.localScale.y), 0, wallLayer);
    }










    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(transform.localScale.x, 0.1f));
        Gizmos.DrawWireCube(wallCheck.position, new Vector3(0.1f, transform.localScale.y));
    }
}
