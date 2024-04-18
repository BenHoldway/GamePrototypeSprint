using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    public PlayerControls PlayerControls {  get; private set; }
    Rigidbody2D rb;

    PlayerMovement movement;
    PlayerGrapple grapple;
    PlayerJump jump;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();

        grapple = GetComponent<PlayerGrapple>();
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();

        grapple.enabled = true;
        movement.enabled = true;
        jump.enabled = true;
    }

    private void OnEnable()
    {
        PlayerControls.Enable();

        UIManager.ChangeGameState += Disable;
        FinishLevel.levelCompleted += Disable;
        AreaManager.ChangeRoom += AreaChange;
    }

    private void OnDisable()
    {
        PlayerControls.Disable();
        UIManager.ChangeGameState -= Disable;
        FinishLevel.levelCompleted -= Disable;
        AreaManager.ChangeRoom -= AreaChange;
    }

    void Disable()
    {
        this.enabled = false;
    }

    void AreaChange(bool _isFirstRoom, Vector2 direction)
    {
        movement.roomChangeVelocity = rb.velocity;

        //Disables input
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        //Gets 2 units in front of where the player is facing, and sets the player's position to this new position
        if (!_isFirstRoom)
        {
            Vector2 newPos = Vector2.zero;

            if (direction == Vector2.up)
                newPos = new Vector2(transform.position.x, transform.position.y + 2f);
            else if (direction != Vector2.down)
                newPos = new Vector2(transform.position.x + (movement.Scale.x * 2f), transform.position.y);

            if(newPos != Vector2.zero)
                transform.position = newPos;
        }

        jump.enabled = false;
        grapple.enabled = false;
        this.enabled = false;
    }
}
