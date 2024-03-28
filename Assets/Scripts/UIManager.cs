using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    PlayerControls playerControls;

    [SerializeField] GameObject pauseMenu;

    public static event Action ChangeGameState;

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
            Debug.Log("Started");
            if (pauseMenu == null)
                return;

            ChangeGameState?.Invoke();
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        };
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
}
