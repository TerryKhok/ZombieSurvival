using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] public static bool isPaused = false;

    public GameObject pauseMenuUI;

    private void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;  //カーソル非表示
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;  //カーソル表示
        Cursor.lockState = CursorLockMode.None;
    }

    public void Playagain()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Time.timeScale = 1f;
        isPaused = false;
        Resume();
        SceneManager.LoadScene("Main");
    }
    // public void WeaponSelect()
    // {
    //     //FindObjectOfType<AudioManager>().Play("ButtonPressed");
    //     Time.timeScale = 1f;
    //     SceneManager.LoadScene("WeaponSelectScene");
    //     isPaused = false;
    // }

    public void loadMenu()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        FindObjectOfType<AudioManager>().Stop("BGM");
        // FindObjectOfType<AudioManager>().Play("BGM1");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
        isPaused = false;
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("Button");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
