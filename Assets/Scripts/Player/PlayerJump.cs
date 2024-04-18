using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PlayerJump : MonoBehaviour
{
    InputManager input;
    PlayerMovement playerMovement;
    AreaChecks areaChecks;

    Rigidbody2D rb;

    #region Jumping
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
 
    bool isOnGround;
    bool isJumping;

    [SerializeField] bool canJump;
    [SerializeField] bool canAirJump;

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
        float wallJumpTimer;
        bool canWallJump;
        public bool IsWallJumping { get; private set; }
        public float WallJumpDir {  get; private set; }
        #endregion
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        areaChecks = GetComponent<AreaChecks>();
        input = GetComponent<InputManager>();
    }

    private void OnEnable()
    {
        input.enabled = true;

        //Jump is performed
        input.PlayerControls.Player.ActionKey.performed += ctx =>
        {
            //Runs if the timer is above 0 and able to wall jump
            if(wallJumpTimer > 0f && canWallJump)
            {
                canWallJump = false;
                IsWallJumping = true;
                rb.AddForce(new Vector2(WallJumpDir * wallJumpPower.x, wallJumpPower.y), ForceMode2D.Impulse);
                wallJumpTimer = 0;

                //StartCoroutine(ReenableInput());

                //Stops the wall jump
                Invoke(nameof(StopWallJump), wallJumpDuration);
            }


            //Makes the player jump if the player isn't wall jumping and can jump normally
            else if(canJump && coyoteTimer > 0f)
            {
                isJumping = true;
                canJump = false;
                Jump();
            }
            else if(canAirJump)
            {
                isJumping = true;
                canAirJump = false;
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
        bool isOnWall = areaChecks.IsOnWall();

        //If player is on wall, not on the ground and falling, set vertical speed to sliding speed
        if (isOnWall && !isOnGround && rb.velocity.y < 0f)
        {
            if (!canWallJump)
            {
                canWallJump = true;
                //Gets the direction of the player, and inverses it
                WallJumpDir = -transform.localScale.x;
                wallJumpTimer = wallJumpMaxTime;
                CancelInvoke(nameof(StopWallJump));
            }

            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -slidingSpeed, 10));
        }
        //Disables the player wall sliding
        else if(wallJumpTimer <= 0f || isOnGround)
            canWallJump = false;
        else if(canWallJump)
            wallJumpTimer -= Time.deltaTime;

        //Handles the gravity depending on if the player is falling or not
        GravityControl();

        //Resets the jumping counter and coyote time
        if (isOnGround && !isJumping)
        {
            canJump = true; 
            canAirJump = true;
            
            coyoteTimer = coyoteMaxTime;
            wallJumpTimer = wallJumpMaxTime;
        }
        //Decreases coyote time when player isn't on the ground
        else if (canJump)
            coyoteTimer -= Time.deltaTime;

    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
    }

    //Stops the player wall jump
    void StopWallJump()
    {
        IsWallJumping = false;
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
        yield return new WaitForSeconds(0.5f);
        input.enabled = true;
    }
}
