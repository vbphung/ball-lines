using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highestScoreText;

    private Score score;

    private void Awake()
    {
        score = FindObjectOfType<Score>();
        score.onScoreUpdated.AddListener(Redraw);
    }

    private void Start()
    {
        Redraw();
    }

    private void Redraw()
    {
        scoreText.text = "Score: " + score.CurrentScore.ToString();
        highestScoreText.text = "Highest Score: " + score.HighestScore.ToString();
    }
}
