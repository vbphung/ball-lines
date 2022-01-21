using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallSlotUI : MonoBehaviour, IDragContainer<Ball>
{
    private Ball ball;
    private BallBoard ballBoard;
    private int index;

    [SerializeField] private BallAvatarUI avatar;
    [SerializeField] private Image avatarInQueue;

    public void Setup(BallBoard ballBoard, int index)
    {
        avatarInQueue.gameObject.SetActive(false);

        this.ballBoard = ballBoard;
        ballBoard.onExplodeBalls.AddListener(ExplodeBall);

        this.index = index;
        ball = ballBoard.GetBallIn1DIndex(index);
        avatar.Setup(ball ? ball.Avatar : null);
    }

    public void SetupInQueue(Ball ball)
    {
        avatar.gameObject.SetActive(false);

        this.ball = ball;
        avatarInQueue.sprite = ball.Avatar;
    }

    private void ExplodeBall(List<int> indecies)
    {
        if (indecies.Contains(index))
        {
            var newExplodeEffect = Instantiate(ballBoard.Level.BallExplodeEffect, transform);
            newExplodeEffect.GetComponent<Image>().color = ball.ExplodeColor;
        }
    }

    public bool Acceptable(int sourceIndex)
    {
        return ball == null && (ballBoard.GetBallIn1DIndex(sourceIndex).Category == BallCategory.Ghost || ballBoard.MoveChecker.CanMoveBall(sourceIndex, index));
    }

    public void AddItems(Ball item)
    {
        ballBoard.AddBallToIndex(item, index);
    }

    public Ball GetItem()
    {
        return ball;
    }

    public void RemoveItems()
    {
        ballBoard.AddBallToIndex(null, index);
    }

    public int GetIndex()
    {
        return index;
    }
}
