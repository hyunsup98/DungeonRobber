using UnityEngine;

// Ʈ�� ���� Ǯ �Ŵ��� (��Ÿ�ӿ�)
[DisallowMultipleComponent]
public class TrapPoolRuntime : ObjectPool<Trap>
{
    // �̱��� �ʱ�ȭ
    void Awake()
    {
        SingletonInit();
    }
}
