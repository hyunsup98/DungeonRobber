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

    // ���� ���� Ǯ
    [SerializeField] private TrapPoolRuntime pool;

    // �̸� ������ ���
    [SerializeField] private Entry[] entries;

    // ���� ������ Ǯ Ž��
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
            Debug.LogWarning("[TrapPoolPrewarm] ���� ������ TrapPoolRuntime�� ã�� �� �����ϴ�.");
        }
    }

    // Ʈ�� �̸� ����
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
