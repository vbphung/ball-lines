using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Level Pack", menuName = "Level Pack", order = 0)]
public class LevelPack : ScriptableObject
{
    [field: SerializeField] public int Row { get; private set; }
    [field: SerializeField] public int Column { get; private set; }
    [field: SerializeField] public float LevelTimeByMinute { get; private set; }
    [field: SerializeField] public Texture2D BackgroundTexture { get; private set; }
    [SerializeField] [Range(0f, 1f)] private float spawnBallRate;
    [SerializeField] private Ball[] normalBalls;
    [SerializeField] private SpecialBall[] specialBalls;
    [field: SerializeField] public ExplodeEffect BallExplodeEffect { get; private set; }

    public string LevelName { get => name; }
    public float LevelTime { get => LevelTimeByMinute * 60; }
    public Sprite Background
    {
        get
        {
            var rect = new Rect(0, 0, BackgroundTexture.width, BackgroundTexture.height);
            var pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(BackgroundTexture, rect, pivot);
        }
    }

    public Ball[,] GetRandomBallBoard()
    {
        Ball[,] ballSlots = new Ball[Row, Column];
        for (int i = 0; i < Row; i++)
            for (int j = 0; j < Column; j++)
                if (ShouldSpawnBall())
                    ballSlots[i, j] = GetRandomBall();
        return ballSlots;
    }

    public SpecialBall GetSpecialBallWhenExplode(int explodeBalls)
    {
        Array.Sort(specialBalls, BallCompare);

        for (int i = 0; i < specialBalls.Length - 1; i++)
            if (explodeBalls >= specialBalls[i].BallsToCraft && explodeBalls < specialBalls[i + 1].BallsToCraft)
                return specialBalls[i];

        return specialBalls[specialBalls.Length - 1];

        int BallCompare(SpecialBall ball1, SpecialBall ball2)
        {
            if (ball1.BallsToCraft > ball2.BallsToCraft)
                return 1;

            return ball1.BallsToCraft < ball2.BallsToCraft ? -1 : 0;
        }
    }

    public Ball GetRandomBall()
    {
        Dictionary<Ball, float> ballRates = new Dictionary<Ball, float>();
        float totalRate = 0;

        foreach (var ball in normalBalls)
        {
            ballRates[ball] = 1;
            totalRate++;
        }

        foreach (var ball in specialBalls)
            if (ball.AutoSpawn)
            {
                ballRates[ball] = ball.SpawnRate;
                totalRate += ball.SpawnRate;
            }

        float spawnRate = Random.Range(0f, totalRate);

        float currentRate = 0;

        foreach (var ballRate in ballRates)
            if (spawnRate >= currentRate && spawnRate <= currentRate + ballRate.Value)
                return ballRate.Key;
            else
                currentRate += ballRate.Value;

        return normalBalls[0];
    }

    private bool ShouldSpawnBall()
    {
        float rateToSpawn = Random.Range(0f, 1f);
        return rateToSpawn > (1 - spawnBallRate);
    }
}
