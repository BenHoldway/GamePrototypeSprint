using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageInput : MonoBehaviour
{
    public PlayerControls PlayerControls {  get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        PlayerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        PlayerControls.Enable();
    }

    private void OnDisable()
    {
        PlayerControls.Disable();
    }
}
