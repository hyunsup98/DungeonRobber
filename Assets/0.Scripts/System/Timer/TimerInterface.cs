using UnityEngine;
public interface IGameTimerObserver //시간변경시 실행되어야하는 메서드 
{
    public void OnTimerChanged(int minute, int hour);
}

public interface IDangerousTimeObserver //위험시간시 실행되어야하는 메서드
{
    public void OnDangerousTimeReached();
}


