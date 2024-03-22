using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

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
    [SerializeField] LayerMask grappleLayer;
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
            Debug.Log("Grapple");
            grapple.enabled = true;
            extendGrapple = true;

            GetRayAngle();

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayAngle, grappleLength);
            if (hit.collider != null)
            {
                grapplePoint = hit.point;

                grapplePoint.z = 0;

                if(hit.collider.gameObject.tag == "Grappable") 
                {
                    hasGrappled = true;
                    joint.connectedAnchor = grapplePoint;
                    joint.distance = grappleLength;
                    joint.enabled = true;
                }
            }
            else
                grapplePoint = transform.position + (rayAngle * grappleLength);


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
        if(grapple.enabled)
        {
            if (!extendGrapple)
            {
                Debug.Log("Retract");
                grappleCurrentTime -= grappleTimeIncrement;
                if(grappleCurrentTime < 0f)
                    FinishGrapple();
            }
            else if (grappleCurrentTime < 1f)
            {
                grappleCurrentTime += grappleTimeIncrement;
                Debug.Log("Extend");
            }
            else if (grappleCurrentTime >= 1f)
            {
                Debug.Log("Grapple Extension Finished");
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

    void GetRayAngle() 
    { 
        rayAngle = new Vector2(transform.position.x + (grappleLength * transform.localScale.x), transform.position.y + grappleLength);
    }

    void GrappleForce() 
    {
        rb.velocity = new Vector2(rayAngle.x * grappleVelocity, rayAngle.y * grappleVelocity);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (grappleLength * transform.localScale.x), transform.position.y + grappleLength));
    }
}
