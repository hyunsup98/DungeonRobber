using UnityEngine;

//리스폰 알림 인터페이스
public interface IRespawnNotifier
{
    void NotifyObjectDestroyed(GameObject obj);
}


//오브젝트 파괴 감지 트래커
public class DestructionTracker : MonoBehaviour
{
    private IRespawnNotifier spawner;

    public void Initialize(IRespawnNotifier spawnerRef)
    {
        spawner = spawnerRef;
    }

    private void OnDestroy()
    {
        MonoBehaviour mb = spawner as MonoBehaviour;
        if (mb != null)
        {
            spawner.NotifyObjectDestroyed(gameObject);
        }
    }
}
