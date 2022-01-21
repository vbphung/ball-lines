using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI timeBoard;

    private Timer timer;

    private void Awake()
    {
        timer = FindObjectOfType<Timer>();
        timer.onTimeChanged.AddListener(Redraw);
    }

    private void Start()
    {
        Redraw();
    }

    private void Redraw()
    {
        timeSlider.value = timer.TimeRemainPercentage;
        timeBoard.text = ConvertToMinute(timer.CurrentTime);

        string ConvertToMinute(float time)
        {
            int minute = Mathf.FloorToInt(time / 60);
            int second = Mathf.FloorToInt(time % 60);

            return (minute < 10 ? "0" : "") + minute.ToString() + ":" + (second < 10 ? "0" : "") + second.ToString();
        }
    }
}
