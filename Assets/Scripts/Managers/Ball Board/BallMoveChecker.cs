using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMoveChecker
{
    private BallBoard ballBoard;

    public BallMoveChecker(BallBoard ballBoard)
    {
        this.ballBoard = ballBoard;
    }

    public bool CanMoveBall(int sourceIndex, int destinationIndex)
    {
        return CanMoveBall2D(ballBoard.To2DPosition(sourceIndex), ballBoard.To2DPosition(destinationIndex));
    }

    private bool CanMoveBall2D(KeyValuePair<int, int> sourcePosition, KeyValuePair<int, int> destinationPosition)
    {
        bool[,] visitedPositions = new bool[ballBoard.Level.Row, ballBoard.Level.Column];
        for (int i = 0; i < ballBoard.Level.Row; i++)
            for (int j = 0; j < ballBoard.Level.Column; j++)
                visitedPositions[i, j] = ballBoard.GetBallIn2DPosition(new KeyValuePair<int, int>(i, j)) != null;

        List<KeyValuePair<int, int>> heap = new List<KeyValuePair<int, int>>();

        heap.Add(sourcePosition);
        visitedPositions[sourcePosition.Key, sourcePosition.Value] = true;

        for (int currentPosition = 0; currentPosition < heap.Count; currentPosition++)
        {
            var u = heap[currentPosition];

            if (u.Key == destinationPosition.Key && u.Value == destinationPosition.Value)
                return true;

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (i == 0 ^ j == 0)
                    {
                        var v = new KeyValuePair<int, int>(u.Key + i, u.Value + j);
                        if (v.Key < 0 || v.Key >= ballBoard.Level.Row || v.Value < 0 || v.Value >= ballBoard.Level.Column || visitedPositions[v.Key, v.Value])
                            continue;

                        visitedPositions[v.Key, v.Value] = true;
                        heap.Add(v);
                    }
        }

        return false;
    }
}
