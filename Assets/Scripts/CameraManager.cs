using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    bool isMovingToNewArea;
    Vector3 areaToMoveTo;

    public static event Action movingFinished;
    public static event Action movePlayerToNewArea;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        AreaTrigger.showNewArea += MoveToNewArea;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMovingToNewArea)
        {
            transform.position = Vector3.Lerp(transform.position, areaToMoveTo, 5f * Time.deltaTime);

            float dist = Vector3.Distance(transform.position, areaToMoveTo);
            if (dist < 5f && dist > 2f)
                movePlayerToNewArea?.Invoke();

            if (transform.position ==  areaToMoveTo)
            {
                Debug.Log("Has moved to new area");
                isMovingToNewArea = false;
                movingFinished?.Invoke();
            }
        }

    }

    void MoveToNewArea(GameObject newArea)
    {
        isMovingToNewArea = true;
        areaToMoveTo = new Vector3(newArea.transform.position.x , newArea.transform.position.y, transform.position.z);
    }
}
