using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaChecks : MonoBehaviour
{
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform leftSideCheck;
    [SerializeField] Transform rightSideCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    public bool IsGrounded()
    {
        return Physics2D.BoxCast(groundCheck.position, new Vector2(transform.localScale.x, 0.05f), 0, -transform.up, groundLayer);
    }

    public bool IsOnWall()
    {
        if (!IsGrounded())
            if (!Physics2D.BoxCast(rightSideCheck.position, new Vector2(0.05f, transform.localScale.y), 0, transform.right, wallLayer))
                return Physics2D.BoxCast(leftSideCheck.position, new Vector2(0.05f, transform.localScale.y), 0, -transform.right, wallLayer);

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, new Vector3(transform.localScale.x, 0.05f, 0.01f));
        Gizmos.DrawWireCube(leftSideCheck.position, new Vector3(0.05f, transform.localScale.y, 0.01f));
        Gizmos.DrawWireCube(rightSideCheck.position, new Vector3(0.05f, transform.localScale.y, 0.01f));
    }
}
