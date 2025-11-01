
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerFunc : Singleton<TimerFunc>
{
    [SerializeField] int startTime;
    [SerializeField] int dangerousTime;
    [SerializeField] float currentTime;

    int minute, hour;
    bool isnotify;

    List<IDangerousTimeObserver> dangerousTimeObserver = new List<IDangerousTimeObserver>();
    List<IGameTimerObserver> gameTimerObserver  = new List<IGameTimerObserver>();

    public void addDangerousTimeEvent(IDangerousTimeObserver observer) => dangerousTimeObserver.Add(observer);
    public void delDangerousTimeEvent(IDangerousTimeObserver observer) => dangerousTimeObserver.Remove(observer);
    public void addTimeChangeEvent(IGameTimerObserver observer) => gameTimerObserver.Add(observer);
    public void delTimeChangeEvent(IGameTimerObserver observer) => gameTimerObserver.Remove(observer);
    
    void Awake()
    {     
        SingletonInit();     
    }

    void OnEnable()
    {
        Init();       
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        NotifyTimechanged();   
    }
    
    void Init()
    {
        currentTime = startTime * 60;
        isnotify = false;
    }

    private void NotifyDangerousTime()
    {
        foreach (var observer in dangerousTimeObserver)
        {
            observer.OnDangerousTimeReached();
        }
    }
    private void NotifyTimechanged()
    {
        minute = (int)currentTime % 60;
        hour = (int)currentTime / 60;

        if (hour == dangerousTime && !isnotify)
        {
            NotifyDangerousTime();
            isnotify = true;
        }
        
        foreach (var observer in gameTimerObserver)
        {
            observer.OnTimerChanged(minute, hour);
        }
    }
}



