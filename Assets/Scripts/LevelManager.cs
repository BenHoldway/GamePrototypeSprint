using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject[] areasInOrder;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < areasInOrder.Length; i++)
        {
            if(i == 0)
                areasInOrder[0].SetActive(true);
            else
                areasInOrder[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
