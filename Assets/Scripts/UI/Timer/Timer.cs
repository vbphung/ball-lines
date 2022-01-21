using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] public UnityEvent onTimeChanged;

    public float TimeRemainPercentage { get => CurrentTime / levelTime; }
    public bool IsStop { get; set; }

    public float CurrentTime
    {
        get => currentTime; private set
        {
            currentTime = value;

            if (currentTime > 0)
                onTimeChanged?.Invoke();
        }
    }

    private float levelTime;
    private float freezeTime;
    private float currentTime;

    public void Setup(float levelTime)
    {
        this.levelTime = levelTime;
        CurrentTime = levelTime;
        IsStop = false;
        freezeTime = 0;
    }

    public void AddFreezeTime(float freezeTime)
    {
        this.freezeTime += freezeTime;
    }

    private void Update()
    {
        if (!IsStop)
        {
            if (freezeTime > 0)
                freezeTime -= Time.deltaTime;
            else
                CurrentTime = Mathf.Max(0, CurrentTime - Time.deltaTime);
        }
    }
}
