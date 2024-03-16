using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    [SerializeField] GameObject newArea;
    [SerializeField] GameObject oldArea;
    public static event Action<GameObject> showNewArea;

    bool loadingNewArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        CameraManager.movingFinished += UnloadOldArea;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UnloadOldArea() 
    {
        if (loadingNewArea)
        {
            oldArea.SetActive(false);
            loadingNewArea = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && !loadingNewArea)
        {
            showNewArea?.Invoke(newArea);
            newArea.SetActive(true);
            loadingNewArea = true;
        }
    }
}
