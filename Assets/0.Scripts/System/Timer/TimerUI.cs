using UnityEngine;
using TMPro;
using System;

/// <summary>
/// 타이머의 UI를 설정해주는 스크립트 
/// </summary>
public class TimerUI: MonoBehaviour,IGameTimerObserver
{
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TimerFunc timerFunc;

    private void Awake()
    {
        timerFunc.addTimeChangeEvent(this);
    }

    private void OnDestroy()
    {
        timerFunc.delTimeChangeEvent(this);
    }

    public void OnTimerChanged(int minute, int hour)
    {
        if (hour >= 12)
        {
            timeText.text = string.Format($"PM {hour:D2}:{minute:D2}");
        }
        else
        {
            timeText.text = string.Format($"AM {hour:D2}:{minute:D2}");            
        }
         
    } 
}
