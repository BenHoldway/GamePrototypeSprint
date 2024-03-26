using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ManageInput))]
public class PlayerJump : MonoBehaviour
{
    ManageInput input;
    PlayerMovement playerMovement;
    AreaChecks areaChecks;

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
    [SerializeField] float slidingSpeed;

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
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        areaChecks = GetComponent<AreaChecks>();
        input = GetComponent<ManageInput>();

        currentJump = 0;
    }

    private void OnEnable()
    {
        input.enabled = true;

        //Jump is performed
        input.PlayerControls.Player.ActionKey.performed += ctx =>
        {
            WallJump();

            //Runs if the timer is above 0 and able to wall jump
            if(wallJumpTimer > 0f)
            {
                isWallJumping = true;
                rb.AddForce(new Vector2(wallJumpDir * wallJumpPower.x, wallJumpPower.y), ForceMode2D.Impulse);
                wallJumpTimer = 0;
                currentJump++;

                //StartCorReenableInput();

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

        input.PlayerControls.Player.ActionKey.canceled += ctx =>
        {
            isJumping = false;
            if (rb.velocity.y > 0f)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            Invoke(nameof(StopWallJump), 0);
        };
    }


    bool onGround;
    // Update is called once per frame
    void Update()
    {
        isOnGround = areaChecks.IsGrounded();

        //If player is on wall, not on the ground and falling, set vertical speed to sliding speed
        if (areaChecks.IsOnWall() && !isOnGround && rb.velocity.y < 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -slidingSpeed, 10));
        }
        //Disables the player wall sliding
        else if(wallJumpTimer <= 0f)
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

        if (isWallSliding)
        {
            if(wallJumpTimer != wallJumpMaxTime)
                wallJumpTimer = wallJumpMaxTime;
            else
                wallJumpTimer -= Time.deltaTime;
        }
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
        if (rb.velocity.y < 0f)
        {
            rb.gravityScale = normalGravityScale * fallMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        //Sets gravity back to normal if player is not falling
        else if (rb.gravityScale != normalGravityScale)
            rb.gravityScale = normalGravityScale;
    }

    public IEnumerator ReenableInput()
    {
        input.enabled = false;
        yield return new WaitForSeconds(1);
        input.enabled = true;
    }
}
