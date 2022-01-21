using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BallBoard : MonoBehaviour
{
    [field: SerializeField] public LevelPack Level { get; private set; }
    [SerializeField] private Image gameBackground;
    [SerializeField] private Image freezeSpawnerBackground;
    [SerializeField] private GameOverPanelUI gameOverPanel;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [field: SerializeField] public BallSpawner Spawner { get; private set; }
    [field: SerializeField] public Timer GameTimer { get; private set; }
    [SerializeField] public UnityEvent onUpdated;
    [SerializeField] public UnityEvent<List<int>> onExplodeBalls;
    [SerializeField] public UnityEvent onLoseGame;

    public BallMoveChecker MoveChecker { get; private set; }

    private Ball[,] ballSlots;
    private FiveMatchChecker matchChecker;
    private LoseGameChecker loseGameChecker;
    private float timeWaitToExplodeBalls;
    private bool isLoseGame;

    private void Awake()
    {
        ballSlots = Level.GetRandomBallBoard();
        gameBackground.sprite = Level.Background;
        freezeSpawnerBackground.enabled = false;
        gameOverPanel.gameObject.SetActive(false);
        levelNameText.text = Level.LevelName.ToUpper();
        Spawner.Setup(this);
        GameTimer.Setup(Level.LevelTime);

        MoveChecker = new BallMoveChecker(this);
        matchChecker = new FiveMatchChecker(this);
        loseGameChecker = new LoseGameChecker(this);

        timeWaitToExplodeBalls = Level.BallExplodeEffect.Duration;
        isLoseGame = false;

        FindObjectOfType<SoundPlayer>().Setup(this);
    }

    private void Update()
    {
        if (!isLoseGame && loseGameChecker.IsOverTime())
        {
            isLoseGame = true;
            gameOverPanel.gameObject.SetActive(true);
            onLoseGame?.Invoke();
        }
    }

    private void Start()
    {
        for (int i = 0; i < Level.Row * Level.Column; ++i)
            if (GetBallIn1DIndex(i) != null)
                matchChecker.DestroyExplodedBalls(matchChecker.GetExplodedBalls(i).Value);
    }

    public Ball GetBallIn1DIndex(int index)
    {
        var positionIn2D = To2DPosition(index);
        return ballSlots[positionIn2D.Key, positionIn2D.Value];
    }

    public Ball GetBallIn2DPosition(KeyValuePair<int, int> position)
    {
        return ballSlots[position.Key, position.Value];
    }

    public void AddBallToIndex(Ball ball, int index)
    {
        SetBallAtPosition(ball, To2DPosition(index));
        StartCoroutine(UpdateBoard(index, true));
    }

    public void SetBallAtPosition(Ball ball, KeyValuePair<int, int> position)
    {
        ballSlots[position.Key, position.Value] = ball;
    }

    private IEnumerator UpdateBoard(int index, bool shouldSpawn)
    {
        onUpdated?.Invoke();

        yield return null;

        if (GetBallIn1DIndex(index) == null)
            yield break;

        var explodeBalls = matchChecker.GetExplodedBalls(index);

        if (explodeBalls.Value.Count > 0)
        {
            onExplodeBalls?.Invoke((from position in explodeBalls.Value
                                    select To1DIndex(position)).ToList());

            yield return new WaitForSeconds(timeWaitToExplodeBalls);

            if (explodeBalls.Key == BallCategory.Freeze)
                GameTimer.AddFreezeTime(10f);
        }
        else if (!isLoseGame && loseGameChecker.IsNotEnoughBallSlot())
        {
            isLoseGame = true;
            gameOverPanel.gameObject.SetActive(true);
            GameTimer.IsStop = true;

            onLoseGame?.Invoke();

            yield break;
        }

        matchChecker.DestroyExplodedBalls(explodeBalls.Value);
        onUpdated?.Invoke();

        SpawnSpecialBall(explodeBalls.Value.Count, To2DPosition(index));

        if (shouldSpawn)
        {
            yield return null;

            var spawnPositions = Spawner.SpawnBalls();
            onUpdated?.Invoke();

            foreach (var spawnPosition in spawnPositions)
            {
                yield return null;
                StartCoroutine(UpdateBoard(To1DIndex(spawnPosition), false));
            }
        }
    }

    private void SpawnSpecialBall(int explodeBalls, KeyValuePair<int, int> position)
    {
        if (explodeBalls > 5)
            SetBallAtPosition(Level.GetSpecialBallWhenExplode(explodeBalls), position);
    }

    public KeyValuePair<int, int> To2DPosition(int index)
    {
        return new KeyValuePair<int, int>(index / Level.Column, index % Level.Column);
    }

    public int To1DIndex(KeyValuePair<int, int> position)
    {
        return position.Key * Level.Column + position.Value;
    }

    public int GetEmptySlots()
    {
        int emptySlots = 0;

        for (int i = 0; i < Level.Row; ++i)
            for (int j = 0; j < Level.Column; ++j)
                if (ballSlots[i, j] == null)
                    emptySlots++;

        return emptySlots;
    }
}
