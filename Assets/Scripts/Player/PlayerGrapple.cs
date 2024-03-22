using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(DistanceJoint2D))]
public class PlayerGrapple : MonoBehaviour
{
    PlayerControls playerControls;
    Rigidbody2D rb;

    Vector3 grapplePoint;
    Vector3 currentGrapplePoint;
    DistanceJoint2D joint;

    [SerializeField] LineRenderer grapple;
    [SerializeField] float grappleLength;
    [SerializeField] LayerMask objectsInLevel;
    [SerializeField] float grappleTimeIncrement;
    [SerializeField] float grappleCurrentTime;
    [SerializeField] float grappleVelocity;
    Vector3 rayAngle;

    bool hasGrappled;
    bool extendGrapple;


    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();

        //Gets joint component and disables it
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        grapple.enabled = false;
    }

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.Player.Grapple.performed += ctx =>
        {
            if (ctx.interaction is HoldInteraction)
            {
                Debug.Log("Hold");
                //rayAngle = new Vector2(transform.position.x + (grappleLength * transform.localScale.x), transform.position.y + grappleLength);
                rayAngle = ReturnRayAngle();

                grapple.enabled = true;
                extendGrapple = true;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayAngle, grappleLength, objectsInLevel);
                if (hit.collider != null)
                {
                    grapplePoint = hit.point;

                    grapplePoint.z = 0;

                    if (hit.collider.gameObject.tag == "Grappable")
                    {
                        hasGrappled = true;
                        joint.connectedAnchor = grapplePoint;
                        joint.distance = grappleLength;
                        joint.enabled = true;
                    }
                }
                else
                    grapplePoint = transform.position + (rayAngle * grappleLength);
            }
        };

        playerControls.Player.Grapple.canceled += ctx =>
        {
            if (joint.enabled)
            {
                FinishGrapple();
            }
        };
    }

    // Update is called once per frame
    void Update()
    {

        if (grapple.enabled)
        {
            if (!extendGrapple)
            {
                grappleCurrentTime -= grappleTimeIncrement;
                if(grappleCurrentTime < 0f)
                    FinishGrapple();
            }
            else if (grappleCurrentTime < 1f)
            {
                grappleCurrentTime += grappleTimeIncrement;
            }
            else if (grappleCurrentTime >= 1f)
            {
                joint.enabled = false;
                extendGrapple = false;

                if (hasGrappled) 
                {
                    GrappleForce();
                }
            }

            currentGrapplePoint = Vector3.Lerp(transform.position, grapplePoint, grappleCurrentTime);
            grapple.SetPosition(1, transform.position);
            grapple.SetPosition(0, currentGrapplePoint);
        }


    }

    Vector3 ReturnRayAngle() 
    { 
        float angle = 0f;

        if (transform.localScale.x == 1f)
            angle = -45f;
        else
            angle = 45f;

        return Quaternion.Euler(0, 0, angle) * Vector3.up;
    }


    void GrappleForce() 
    {
        //rb.velocity = new Vector2(rayAngle.x * grappleVelocity, rayAngle.y * grappleVelocity);
        rb.AddForce(new Vector2(transform.localScale.x, 1) * grappleVelocity, ForceMode2D.Impulse);

        //rb.AddForce(new Vector2(rayAngle.x * grappleVelocity, rayAngle.y * grappleVelocity));
        Debug.Log(new Vector2(rayAngle.x * grappleVelocity, rayAngle.y * grappleVelocity));
        hasGrappled = false;
    }

    void FinishGrapple()
    {
        grapple.enabled = false;

        grappleCurrentTime = 0f;
        extendGrapple = false;
    }






    private void OnDrawGizmos()
    {
        rayAngle = ReturnRayAngle();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (rayAngle * grappleLength));
    }
}
