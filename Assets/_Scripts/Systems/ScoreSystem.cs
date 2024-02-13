using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreSystem : MonoBehaviour
{
    public static int Score;

    private int textScore;
    string _string;
    Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
        Score = 0;
        _string = "SCORE: " + Score.ToString("00000000");
        _text.text = _string;
    }

    private void FixedUpdate()
    {
        if (textScore < Score)
        {
            textScore += 10;
            _string = "SCORE: " + textScore.ToString("00000000");
            _text.text = _string;
        }
    }

    public void AddScore(int score)
    {
        Score += score;
    }
}
