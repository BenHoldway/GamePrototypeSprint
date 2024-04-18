using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    float timerTime;
    int collectableAmount;
    int collectablesCollected;
    int deathCount;

    private void Awake()
    {
        collectableAmount = GameObject.FindObjectsOfType(typeof(Collectable)).Length;

        if(Time.timeScale != 1f)
            Time.timeScale = 1f;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        Collectable.collectableCollected += UpdateCollectableAmount;
        PlayerDeath.death += UpdateDeathCount;
        UIManager.levelCompleted += Display;
    }

    void OnDisable()
    {
        Collectable.collectableCollected -= UpdateCollectableAmount;
        PlayerDeath.death -= UpdateDeathCount;
        UIManager.levelCompleted -= Display;
    }

    // Update is called once per frame
    void Update()
    {
        timerTime += Time.deltaTime;
    }

    void UpdateCollectableAmount()
    {
        collectablesCollected++;
    }

    void UpdateDeathCount()
    {
        deathCount++;
    }

    void Display(TMP_Text timeText, TMP_Text collectableText, TMP_Text deathText)
    {
        timeText.text = $"Time of Completion: {TimeSpan.FromSeconds(timerTime).ToString(@"mm\:ss\.ff")}";
        collectableText.text = $"Collectables Collected: x{collectablesCollected}/{collectableAmount}";
        deathText.text = $"Death Count: x{deathCount}";
    }
}
