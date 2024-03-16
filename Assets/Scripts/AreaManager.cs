using System.Collections;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [SerializeField] GameObject[] adjacentAreas;
    [SerializeField] int areaNum;
    [SerializeField] GameObject[] roomTriggers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(EnableTriggerTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EnableTriggerTimer() 
    {
        EnableTriggers(false);
        yield return new WaitForSeconds(1);
        EnableTriggers(true);
    }

    void EnableTriggers(bool enable) 
    { 
        foreach(GameObject trigger in roomTriggers) 
        { 
            trigger.SetActive(enable);
        }
    }
}
