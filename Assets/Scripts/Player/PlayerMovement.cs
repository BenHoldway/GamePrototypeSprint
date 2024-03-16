using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float speed;
    bool isFacingRight;
    bool isMoving;
    Vector3 scale;
    Vector2 movement;

    PlayerControls playerControls;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isFacingRight = true;
        scale = transform.localScale;

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Move.started += ctx => 
        {
            movement = ctx.ReadValue<Vector2>();
        };
        playerControls.Player.Move.canceled += ctx =>
        {
            movement = ctx.ReadValue<Vector2>();
        };

        playerControls.Player.Crouch.started += ctx =>
        {
            scale.y *= 0.5f;
            transform.localScale = scale;
            Vector2 pos = transform.localPosition;
            pos.y -= 0.25f;
            transform.localPosition = pos;
        };
        playerControls.Player.Crouch.canceled += ctx =>
        {
            scale.y *= 2.0f;
            transform.localScale = scale;
            Vector2 pos = transform.localPosition;
            pos.y += 0.25f;
            transform.localPosition = pos;
        };

        AreaTrigger.showNewArea += GoToNewArea;
        CameraManager.movePlayerToNewArea += AreaLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        if(movement.x > 0.02f || movement.x < 0.02f)
            rb.velocity = new Vector2 (movement.x * speed, rb.velocity.y);

        if ((isFacingRight && movement.x < 0) || (!isFacingRight && movement.x > 0))
            Flip();
    }

    void Flip() 
    { 
        isFacingRight = !isFacingRight;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    void GoToNewArea(GameObject newArea) 
    {
        playerControls.Disable();
    }

    void AreaLoaded() 
    { 
        Vector2 dir = Quaternion.AngleAxis(30, transform.forward) * transform.right;
        rb.AddForce(dir * 5f, ForceMode2D.Force);
        playerControls.Enable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(30, transform.forward) * transform.up * 10);
    }
}
