
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerFunc : Singleton<TimerFunc> //싱글톤으로 구현 
{
    [SerializeField] int startTime; 
    [SerializeField] int dangerousTime; 
    [SerializeField] float currentTime; 

    int minute, hour; 
    bool isnotify; //위험 시간 알림 했는지

    List<IDangerousTimeObserver> dangerousTimeObserver = new List<IDangerousTimeObserver>(); //위험 시간일떄 하는 행동리스트
    List<IGameTimerObserver> gameTimerObserver  = new List<IGameTimerObserver>(); //시간 변경시 하는 행동리스트

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
        currentTime = startTime * 60; //시작 시간 초기화 
        isnotify = false;
    }

    private void NotifyDangerousTime() //위험 시간 알림 
    {
        foreach (var observer in dangerousTimeObserver)
        {
            observer.OnDangerousTimeReached();
        }
    }
    private void NotifyTimechanged() //시간 변경 알림 
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



