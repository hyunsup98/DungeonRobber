using UnityEngine;

// 트랩 전용 풀 매니저 (런타임용)
[DisallowMultipleComponent]
public class TrapPoolRuntime : ObjectPool<Trap>
{
    // 싱글턴 초기화
    void Awake()
    {
        SingletonInit();
    }
}
