using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageScene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
        FindObjectOfType<AudioManager>().Stop("MinigunShot");
        FindObjectOfType<AudioManager>().Stop("BGM");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void Result(){
        SceneManager.LoadScene("Result");
    }
}