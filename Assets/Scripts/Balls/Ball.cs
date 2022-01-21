using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Normal Ball", menuName = "Balls/Normal Ball", order = 0)]
public class Ball : ScriptableObject
{
    [field: SerializeField] public Sprite Avatar { get; private set; }
    [field: SerializeField] public BallCategory Category { get; private set; }
    [field: SerializeField] public Color ExplodeColor { get; private set; }
}
