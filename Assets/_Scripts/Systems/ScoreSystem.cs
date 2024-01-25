using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreSystem : MonoBehaviour
{
    public static int Score;
    string _string;
    Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
        Score = 0;
    }

    private void Update()
    {
        _string = "SCORE: " + Score.ToString("00000000");
        _text.text = _string;
    }

    public void AddScore(int score)
    {
        Score += score;
    }
}
