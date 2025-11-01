using UnityEngine;

[DisallowMultipleComponent]
public class TrapPoolPrewarm : MonoBehaviour
{
    [System.Serializable]
    public struct Entry
    {
        public Trap prefab;
        public int count;
    }

    // 같은 씬의 풀
    [SerializeField] private TrapPoolRuntime pool;

    // 미리 생성할 목록
    [SerializeField] private Entry[] entries;

    // 같은 씬에서 풀 탐색
    void Awake()
    {
        if (pool == null)
        {
            TrapPoolRuntime[] pools = FindObjectsOfType<TrapPoolRuntime>(true);
            foreach (var p in pools)
            {
                if (p.gameObject.scene == gameObject.scene)
                {
                    pool = p;
                    break;
                }
            }
        }

        if (pool == null)
        {
            Debug.LogWarning("[TrapPoolPrewarm] 같은 씬에서 TrapPoolRuntime을 찾을 수 없습니다.");
        }
    }

    // 트랩 미리 생성
    void Start()
    {
        if (pool == null)
        {
            return;
        }

        if (entries == null)
        {
            return;
        }

        foreach (var e in entries)
        {
            if (e.prefab == null)
            {
                continue;
            }

            if (e.count <= 0)
            {
                continue;
            }

            for (int i = 0; i < e.count; i++)
            {
                Trap t = pool.GetObjects(e.prefab, pool.transform);
                pool.TakeObjects(t);
            }
        }
    }
}
