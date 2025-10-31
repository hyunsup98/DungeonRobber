using UnityEngine;

//������ �˸� �������̽�
public interface IRespawnNotifier
{
    void NotifyObjectDestroyed(GameObject obj);
}


//������Ʈ �ı� ���� Ʈ��Ŀ
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
