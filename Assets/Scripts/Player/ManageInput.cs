using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageInput : MonoBehaviour
{
    public PlayerControls PlayerControls {  get; private set; }

    PlayerMovement movement;
    PlayerGrapple grapple;
    PlayerJump jump;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerControls = new PlayerControls();

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
    }

    private void OnDisable()
    {
        PlayerControls.Disable();
        UIManager.ChangeGameState -= Disable;
    }

    void Disable()
    {
        this.enabled = false;
    }
}
