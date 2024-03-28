using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    CinemachineBrain camBrain;
    bool isMovingToNewArea;

    public static event Action RoomSuccessfullyChanged;

    // Start is called before the first frame update
    void Start()
    {
        camBrain = GetComponent<CinemachineBrain>();
    }

    private void OnEnable()
    {
        AreaManager.ChangeRoom += StartCamChange;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isMovingToNewArea && !camBrain.IsBlending)
        {
            isMovingToNewArea = false;
            StartCoroutine(FinishBlend());
        }

    }

    private void StartCamChange(bool _isFirstRoom, Vector2 ignore)
    {
        isMovingToNewArea = true;
    }

    IEnumerator FinishBlend() 
    {
        yield return new WaitForSeconds(0.75f);
        RoomSuccessfullyChanged?.Invoke();
    }
}
