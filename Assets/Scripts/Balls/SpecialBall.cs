using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special Ball", menuName = "Balls/Special Ball", order = 0)]
public class SpecialBall : Ball
{
    [field: SerializeField] public int BallsToCraft { get; private set; }
    [field: SerializeField] public bool AutoSpawn { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float SpawnRate { get; private set; }
}
