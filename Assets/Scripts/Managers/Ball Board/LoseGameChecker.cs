using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseGameChecker
{
    private BallBoard BallBoard;

    public LoseGameChecker(BallBoard ballBoard)
    {
        this.BallBoard = ballBoard;
    }

    public bool IsNotEnoughBallSlot()
    {
        return BallBoard.GetEmptySlots() <= 5;
    }

    public bool IsOverTime()
    {
        return BallBoard.GameTimer.CurrentTime <= 0.1;
    }
}
