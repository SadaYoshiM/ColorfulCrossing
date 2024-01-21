using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text ScoreText;
    public Text HighScoreText;

    private int score;
    private int highScore;
    private float time;
    private float flashSpeed = 5.0f;
    private Color textColor;
    private string textColorCode;

    private string highScoreKey;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if(highScore < score)
        {
            textColor = Flash(textColor);
            textColorCode = UnityEngine.ColorUtility.ToHtmlStringRGB(textColor);
        }
        ScoreText.text = "Score : <color=#" + textColorCode + ">" + score.ToString() + "</color>";
        HighScoreText.text = "High Score : " + highScore.ToString();
    }

    void Initialize()
    {
        score = 0;
        time = 0;
        textColor = Color.white;
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        ScoreText.text = score.ToString();
    }

    public void AddScore(int point, int combo)
    {
        int comboScore = ComboPoint(combo);
        score += point + comboScore;
        Debug.Log("Add Score : " + (point + comboScore).ToString());
    }

    int ComboPoint(int combo)
    {
        if(combo > 0 && combo % 10 == 0)
        {
            Debug.Log(combo.ToString() + "Combo!");
            return combo * 10;
        }
        return combo;
    }

    Color Flash(Color color)
    {
        time += Time.deltaTime * flashSpeed;
        color.g = Mathf.Sin(time);
        color.b = Mathf.Sin(time);
        return color;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(highScoreKey, highScore);
        PlayerPrefs.Save();
    }
}
