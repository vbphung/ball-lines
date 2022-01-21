using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public Dictionary<KeyValuePair<int, int>, Ball> BallQueue { get; private set; }

    private BallBoard ballBoard;

    public void Setup(BallBoard ballBoard)
    {
        this.ballBoard = ballBoard;

        ResetBallQueue();
    }

    public List<KeyValuePair<int, int>> SpawnBalls()
    {
        List<KeyValuePair<int, int>> spawnPositions = new List<KeyValuePair<int, int>>();

        foreach (var ball in BallQueue)
        {
            ballBoard.SetBallAtPosition(ball.Value, ball.Key);
            spawnPositions.Add(ball.Key);
        }

        ResetBallQueue();

        return spawnPositions;
    }

    private KeyValuePair<int, int> GetRandomPosition()
    {
        int row = Random.Range(0, ballBoard.Level.Row);
        int column = Random.Range(0, ballBoard.Level.Column);
        var position = new KeyValuePair<int, int>(row, column);

        while (ballBoard.GetBallIn2DPosition(position) != null || BallQueue.ContainsKey(position))
        {
            row = Random.Range(0, ballBoard.Level.Row);
            column = Random.Range(0, ballBoard.Level.Column);
            position = new KeyValuePair<int, int>(row, column);
        }

        return position;
    }

    private void ResetBallQueue()
    {
        BallQueue = new Dictionary<KeyValuePair<int, int>, Ball>();

        for (int i = 0; i < 3; ++i)
            BallQueue[GetRandomPosition()] = ballBoard.Level.GetRandomBall();
    }
}
