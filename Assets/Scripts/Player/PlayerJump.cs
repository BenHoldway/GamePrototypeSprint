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

    float coyoteTime = 0.2f;
    float coyoteTimeCounter;
    #endregion

    #region WallSliding
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask wallLayer;

    bool isWallSliding;

        #region WallJumping
        [SerializeField] Vector2 wallJumpPower;
        [SerializeField] float wallJumpTime;
        [SerializeField] float wallJumpDuration;
        bool isWallJumping;
        float wallJumpDir;
        float wallJumpCounter;
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

        playerControls.Player.ActionKey.performed += ctx =>
        {
            WallJump();

            if(wallJumpCounter > 0f)
            {
                if(transform.localScale.x != wallJumpDir)
                {
                    playerMovement.Flip();
                }
                isWallJumping = true;
                rb.velocity = new Vector2(wallJumpDir * wallJumpPower.x, wallJumpPower.y);
                Debug.Log("Jump");
                wallJumpCounter = 0;


                Invoke(nameof(StopWallJump), wallJumpDuration);
            }

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

        if (IsOnWall() && !isOnGround && rb.velocity.y < 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        else
            isWallSliding = false;

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
        //rb.AddForce(Vector2.up * jumpingPower, ForceMode2D.Impulse);
        currentJump++;
    }

    private void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = true;
            //Gets the direction of the player, and inverses it
            wallJumpDir = -transform.localScale.x;
            wallJumpCounter = wallJumpTime;
            CancelInvoke(nameof(StopWallJump));
        }
        else
            wallJumpCounter -= Time.deltaTime;
    }

    void StopWallJump()
    {
        isWallJumping = false;
    }

    void GravityControl() 
    { 
        if(rb.velocity.y < 0f) 
        { 
            rb.gravityScale = normalGravityScale * fallMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else if(rb.gravityScale != normalGravityScale)
            rb.gravityScale = normalGravityScale;
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, new Vector2(transform.localScale.x, 0.1f), 0, groundLayer);
    }

    bool IsOnWall() 
    {
        return Physics2D.OverlapBox(wallCheck.position, new Vector2(transform.localScale.x, 0.1f), 0, wallLayer);
    }










    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(transform.localScale.x, 0.1f));
       // Gizmos.DrawWireCube(leftSideCheck.position - transform.right * 0.025f, new Vector3(0.05f, transform.localScale.y, 0.01f));
       // Gizmos.DrawWireCube(rightSideCheck.position + transform.right * 0.025f, new Vector3(0.05f, transform.localScale.y, 0.01f));
    }
}
