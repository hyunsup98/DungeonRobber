using UnityEngine;
public interface IGameTimerObserver
{
    public void OnTimerChanged(int minute, int hour);
}

public interface IDangerousTimeObserver
{
    public void OnDangerousTimeReached();
}


