using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    PlayerControls playerControls;

    [SerializeField] GameObject pauseMenu;

    [SerializeField] GameObject endScreen;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text collectablesText;
    [SerializeField] TMP_Text deathText;

    public static event Action ChangeGameState;
    public static event Action<TMP_Text, TMP_Text, TMP_Text> levelCompleted;

    // Start is called before the first frame update
    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            return;

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            return;

        playerControls.Enable();

        playerControls.Player.Pause.started += ctx =>
        {
            if (pauseMenu == null)
                return;

            ChangeGameState?.Invoke();
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        };

        FinishLevel.levelCompleted += ShowEndScreen;
    }

    private void OnDisable()
    {
        FinishLevel.levelCompleted -= ShowEndScreen;
    }

    public void LoadGame() 
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMenu() 
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        ChangeGameState?.Invoke();
    }

    void ShowEndScreen()
    {
        endScreen.SetActive(true);
        levelCompleted?.Invoke(timeText, collectablesText, deathText);
    }
}
