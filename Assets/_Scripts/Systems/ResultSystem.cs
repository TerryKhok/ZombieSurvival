using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSystem : MonoBehaviour
{
    int _result;
    string _string;
    Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<Text>();
        _result = ScoreSystem.Score;
        _string = "SCORE: " + _result.ToString("00000000");
        _text.text = _string;
    }

}
