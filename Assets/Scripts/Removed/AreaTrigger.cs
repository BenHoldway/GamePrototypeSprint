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
        DetectNewArea.GoToNewArea += Trigger;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Runs when the GoToNewArea event is called
    void Trigger(GameObject _trigger)
    {
        //Checks to see if the trigger triggered is this same one
        if(gameObject.name == _trigger.name && !loadingNewArea) 
        {
            //Calls event, and reveals new area
            showNewArea?.Invoke(newArea);
            newArea.SetActive(true);
            loadingNewArea = true;
        }
    }

    void UnloadOldArea() 
    {
        //Will only run if new area is being loaded
        if (loadingNewArea)
        {
            //Hides old area
            oldArea.SetActive(false);
            loadingNewArea = false;
        }
    }

/*    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && !loadingNewArea)
        {
            showNewArea?.Invoke(newArea, false);
            newArea.SetActive(true);
            loadingNewArea = true;
        }
    }*/
}
