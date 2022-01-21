using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiveMatchChecker
{
    private BallBoard ballBoard;

    public FiveMatchChecker(BallBoard ballBoard)
    {
        this.ballBoard = ballBoard;
    }

    public void DestroyExplodedBalls(List<KeyValuePair<int, int>> explodeBalls)
    {
        if (explodeBalls.Count == 0)
            return;

        foreach (var ballPosition in explodeBalls)
            ballBoard.SetBallAtPosition(null, ballPosition);
    }

    public KeyValuePair<BallCategory, List<KeyValuePair<int, int>>> GetExplodedBalls(int index)
    {
        var positionIn2D = ballBoard.To2DPosition(index);

        var explodeBallGroup = GetLargestBallGroup(new KeyValuePair<BallCategory, List<KeyValuePair<int, int>>>[]
        {
            CheckByWay(positionIn2D, MatchWay.Horizontal),
            CheckByWay(positionIn2D, MatchWay.Vertical),
            CheckByWay(positionIn2D, MatchWay.TopLeft),
            CheckByWay(positionIn2D, MatchWay.TopRight)
        });


        if (explodeBallGroup.Value.Count > 0)
            explodeBallGroup.Value.Add(ballBoard.To2DPosition(index));

        return explodeBallGroup;
    }

    private KeyValuePair<BallCategory, List<KeyValuePair<int, int>>> CheckByWay(KeyValuePair<int, int> ballPosition, MatchWay matchWay)
    {
        var checkedWay = GetWay(matchWay);

        var line = GetLargestBallGroup(new KeyValuePair<BallCategory, List<KeyValuePair<int, int>>>[]
        {
            CheckByDirection(ballPosition, checkedWay.Key, checkedWay.Value),
            CheckByDirection(ballPosition, -checkedWay.Key, -checkedWay.Value)
        });

        if (line.Value.Count >= 4)
            return line;

        return new KeyValuePair<BallCategory, List<KeyValuePair<int, int>>>(BallCategory.Rainbow, new List<KeyValuePair<int, int>>());
    }

    private KeyValuePair<BallCategory, List<KeyValuePair<int, int>>> GetLargestBallGroup(KeyValuePair<BallCategory, List<KeyValuePair<int, int>>>[] lines)
    {
        var oftenCategory = GetOftenCategory(lines);

        var ballGroups = new List<KeyValuePair<int, int>>();

        foreach (var line in lines)
            if (line.Key == oftenCategory)
                ballGroups.AddRange(line.Value);

        return new KeyValuePair<BallCategory, List<KeyValuePair<int, int>>>(oftenCategory, ballGroups);
    }

    private BallCategory GetOftenCategory(KeyValuePair<BallCategory, List<KeyValuePair<int, int>>>[] lines)
    {
        var oftenCategory = BallCategory.Rainbow;
        int oftenTime = 0;

        foreach (var line in lines)
            if (line.Value.Count >= oftenTime)
            {
                oftenTime = line.Value.Count;
                oftenCategory = line.Key;
            }

        return oftenCategory;
    }

    private KeyValuePair<BallCategory, List<KeyValuePair<int, int>>> CheckByDirection(KeyValuePair<int, int> ballPosition, int moveRow, int moveColumn)
    {
        var ball = ballBoard.GetBallIn2DPosition(ballPosition);
        var currentCategory = ball.Category;

        var directionIndecies = new List<KeyValuePair<int, int>>();

        var checkedPosition = GetPositionToCheck(ballPosition, moveRow, moveColumn);
        var checkedBall = GetBallToCheck(checkedPosition);

        while (checkedBall != null && (checkedBall.Category == currentCategory || checkedBall.Category == BallCategory.Rainbow || currentCategory == BallCategory.Rainbow))
        {
            if (checkedBall.Category != BallCategory.Rainbow && currentCategory == BallCategory.Rainbow)
                currentCategory = checkedBall.Category;

            directionIndecies.Add(checkedPosition);
            checkedPosition = GetPositionToCheck(checkedPosition, moveRow, moveColumn);
            checkedBall = GetBallToCheck(checkedPosition);
        }

        return new KeyValuePair<BallCategory, List<KeyValuePair<int, int>>>(currentCategory, directionIndecies);
    }

    private Ball GetBallToCheck(KeyValuePair<int, int> position)
    {
        if (position.Key < 0 || position.Value < 0)
            return null;

        if (position.Key >= ballBoard.Level.Row || position.Value >= ballBoard.Level.Column)
            return null;

        return ballBoard.GetBallIn2DPosition(position);
    }

    private KeyValuePair<int, int> GetPositionToCheck(KeyValuePair<int, int> currentPosition, int moveRow, int moveColumn)
    {
        return new KeyValuePair<int, int>(currentPosition.Key + moveRow, currentPosition.Value + moveColumn);
    }

    private KeyValuePair<int, int> GetWay(MatchWay matchWay)
    {
        switch (matchWay)
        {
            case MatchWay.Horizontal:
                return new KeyValuePair<int, int>(0, 1);
            case MatchWay.Vertical:
                return new KeyValuePair<int, int>(1, 0);
            case MatchWay.TopLeft:
                return new KeyValuePair<int, int>(-1, 1);
            default:
                return new KeyValuePair<int, int>(1, 1);
        }
    }
}

public enum MatchWay
{
    Horizontal, Vertical, TopLeft, TopRight
}
