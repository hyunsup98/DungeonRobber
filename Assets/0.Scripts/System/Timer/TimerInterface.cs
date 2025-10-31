using UnityEngine;
public interface IGameTimerObserver
{
    public void OnTimerChanged(float currentTime);
}

public interface IDangerousTimeObserver
{
    public void OnDangerousTimeReached();
}


