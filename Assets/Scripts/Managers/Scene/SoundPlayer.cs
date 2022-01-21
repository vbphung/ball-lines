using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource explodeSoundPlayer;
    [SerializeField] private AudioSource loseGameSoundPlayer;

    private void Awake()
    {
        if (FindObjectsOfType<SoundPlayer>().Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    public void Setup(BallBoard ballBoard)
    {
        if (ballBoard != null)
        {
            ballBoard.onExplodeBalls.AddListener(PlayExplodeSound);
            ballBoard.onLoseGame.AddListener(PlayLoseGameSound);
        }
    }

    private void PlayExplodeSound(List<int> indecies)
    {
        if (indecies.Count >= 5)
            explodeSoundPlayer.Play();
    }

    private void PlayLoseGameSound()
    {
        loseGameSoundPlayer.Play();
    }
}
