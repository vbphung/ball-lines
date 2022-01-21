using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Score : MonoBehaviour
{
    [SerializeField] public UnityEvent onScoreUpdated;

    public int CurrentScore
    {
        get => score; set
        {
            score = value;
            HighestScore = Mathf.Max(HighestScore, value);

            onScoreUpdated?.Invoke();
        }
    }

    public int HighestScore { get; private set; }

    private int score;
    private BallBoard ballBoard;

    private void Awake()
    {
        ballBoard = FindObjectOfType<BallBoard>();
        ballBoard.onExplodeBalls.AddListener(UpdateScore);
    }

    private void UpdateScore(List<int> indecies)
    {
        CurrentScore += indecies.Count;
    }
}
