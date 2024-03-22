using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AreaChecks : MonoBehaviour
{
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    public bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, new Vector2(transform.localScale.x, 0.1f), 0, groundLayer);
    }

    //Checks if player is up against the wall
    public bool IsOnWall()
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
