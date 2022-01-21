using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallBoardUI : MonoBehaviour
{
    [SerializeField] private BallSlotUI ballSlotPrefab;
    [SerializeField] private Button startMenuButton;

    private BallBoard ballBoard;

    private void Awake()
    {
        ballBoard = FindObjectOfType<BallBoard>();
        ballBoard.onUpdated.AddListener(Redraw);
        ballBoard.onLoseGame.AddListener(delegate { startMenuButton.gameObject.SetActive(false); });

        startMenuButton.onClick.AddListener(delegate { FindObjectOfType<SceneLoader>().LoadStartMenu(); });
    }

    private void Start()
    {
        LevelPack levelPack = ballBoard.Level;

        GridLayoutGroup layoutGroup = GetComponent<GridLayoutGroup>();
        layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layoutGroup.constraintCount = levelPack.Column;

        Redraw();
    }

    private void Redraw()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int i = 0; i < ballBoard.Level.Row * ballBoard.Level.Column; ++i)
        {
            BallSlotUI slot = Instantiate(ballSlotPrefab, transform);

            var ballPosition = ballBoard.To2DPosition(i);

            if (ballBoard.Spawner.BallQueue.ContainsKey(ballPosition))
                slot.SetupInQueue(ballBoard.Spawner.BallQueue[ballPosition]);
            else
                slot.Setup(ballBoard, i);
        }
    }
}
