using UnityEngine;


public class PlayerGrapple : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerMovement move;
    Rigidbody2D rb;

    float normalGravity;
    [SerializeField] float grapplingGravity;

    Vector3 grapplePoint;
    Vector3 currentGrapplePoint;

    Vector3 rayAngle;
    
    [SerializeField] LineRenderer grapple;
    [SerializeField] LineRenderer grappleAim;
    [SerializeField] LayerMask objectsInLevel;

    [SerializeField] float grappleShootTimeIncrement;
    [SerializeField] float grappleCurrentShootTime;

    [SerializeField] float grappleLength;
    [SerializeField] float grappleVelocity;

    [SerializeField] float grappleMaxTime;
    float grappleCurrentTime;
    bool HasGrappled;

    bool aimingGrapple;

    public bool IsGrappling { get; private set; }

    bool extendGrapple;


    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
        move = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        normalGravity = rb.gravityScale;

        grapple.enabled = false;
        grappleAim.enabled = false;
    }

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.Player.Grapple.started += ctx =>
        {
            //Slows down time
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = 0.004f;

            aimingGrapple = true;
        };

        playerControls.Player.Grapple.canceled += ctx =>
        {
            //Sets time back to normal
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            rayAngle = ReturnRayAngle();

            grappleAim.enabled = false;
            grapple.enabled = true;
            extendGrapple = true;

            aimingGrapple = false;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayAngle, grappleLength, objectsInLevel);
            if (hit.collider != null)
            {
                grapplePoint = hit.point;

                grapplePoint.z = 0;

                if (hit.collider.gameObject.tag == "Grappable")
                    HasGrappled = true;
            }
            else
                grapplePoint = transform.position + (rayAngle * grappleLength);
        };

        PlayerDeath.death += FinishGrapple;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        PlayerDeath.death -= FinishGrapple;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Shows the aim for the grapple when the action button is held down
        if(aimingGrapple)
        {
            grappleAim.SetPosition(1, transform.position);
            grappleAim.SetPosition(0, transform.position + (rayAngle * grappleLength));

            grappleAim.enabled = true;
        }
        //Runs if the actual grapple is enabled
        else if (grapple.enabled)
        {
            rb.gravityScale = grapplingGravity;

            //Retracts the grapple back
            if (!extendGrapple)
            {
                grappleCurrentShootTime -= grappleShootTimeIncrement;
                if(grappleCurrentShootTime < 0f)
                    FinishGrapple();
            }
            //Extends the grapple outwards
            else if (grappleCurrentShootTime < 1f)
            {
                grappleCurrentShootTime += grappleShootTimeIncrement;
            }
            //Grapple has fully extended
            else if (grappleCurrentShootTime >= 1f)
            {
                extendGrapple = false;

                //If it has grappled onto something successfully, force will be applied
                if (HasGrappled) 
                    GrappleForce();
            }

            currentGrapplePoint = Vector3.Lerp(transform.position, grapplePoint, grappleCurrentShootTime);
            grapple.SetPosition(1, transform.position);
            grapple.SetPosition(0, currentGrapplePoint);
        }


        //Will run if grapple has hit grappable object
        if (IsGrappling)
        {
            grappleCurrentTime -= Time.deltaTime;

            if (grappleCurrentTime <= 0f)
            {
                IsGrappling = false;
                HasGrappled = false;
            }
        }
    }



    void GrappleForce() 
    {
        move.enabled = false;
        rb.gravityScale = 0f;

        grappleCurrentTime = grappleMaxTime;
        IsGrappling = true;
        rb.AddForce(new Vector2(transform.localScale.x, 1) * grappleVelocity, ForceMode2D.Impulse);


        move.enabled = true;
    }

    void FinishGrapple()
    {
        grapple.enabled = false;
        grappleAim.enabled = false;

        grappleCurrentShootTime = 0f;
        extendGrapple = false;

        if(!move.enabled)
            move.enabled = true;

        rb.gravityScale = normalGravity;
    }






    //Returns the angle to shoot the grapple at
    Vector3 ReturnRayAngle() 
    { 
        float angle = 0f;

        if (transform.localScale.x == 1f)
            angle = -45f;
        else
            angle = 45f;

        return (Quaternion.Euler(0, 0, angle) * Vector3.up);
    }



    private void OnDrawGizmos()
    {
        rayAngle = ReturnRayAngle();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (rayAngle * grappleLength));
    }
}
