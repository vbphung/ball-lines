using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallQueueUI : MonoBehaviour
{
    private BallBoard ballBoard;
    private Image[] ballAvatars;

    private void Awake()
    {
        ballBoard = FindObjectOfType<BallBoard>();
        ballBoard.onUpdated.AddListener(Redraw);

        ballAvatars = GetComponentsInChildren<Image>();
    }

    private void Start()
    {
        Redraw();
    }

    private void Redraw()
    {
        int i = 0;
        foreach (var ball in ballBoard.Spawner.BallQueue)
            ballAvatars[++i].sprite = ball.Value.Avatar;
    }
}
