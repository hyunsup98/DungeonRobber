using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �⺻ ���� ������ ���� (�߻�Ŭ����)
[System.Serializable]
public abstract class SpawnData
{
    public GameObject mapPrefab;
    [Range(0f, 1f)] public float _spawnProbability = 1f;
}


// ������ ������
[System.Serializable] public class ItemSpawnData : SpawnData {}


// ���� ������ 
[System.Serializable] public class MonsterData : SpawnData {}



//������ ������Ʈ�� �ı��� �� �����ʿ� �˸��� �ֱ� ���� �������̽�
public interface IRespawnNotifier
{
    void NotifyObjectDestroyed(GameObject obj);
}


//������ (���׸�)
public abstract class Spawner<T> : MonoBehaviour,
IRespawnNotifier
where T : SpawnData
{

    [Header("���� ����")]
    [SerializeField] private List<T> _spawnDataList = new List<T>();
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _CreateCount;
    [SerializeField] private Vector2 _mapSwpanSize;
    [SerializeField] private float _minSpawnDistance;
    [Range(0f, 1f)][SerializeField] private float _spawnCountChance = 1f;

    [Header("������ ����")]
    [SerializeField] protected bool enableRespawn = true;
    [SerializeField] protected float respawnDelay = 5f;


    private float _raycastHeight = 100f;

    //���� ��ġ ��ħ ������
    private List<Vector3> spawnedPosition = new List<Vector3>();

    //������ ������Ʈ�� ���� ������ ����
    protected Dictionary<GameObject, T> spawnedObjects = new Dictionary<GameObject, T>();


    private void Start()
    {
        InitialSpawn();
    }


    // ���� �� ���� ������ŭ ����
    protected virtual void InitialSpawn()
    {
        spawnedObjects.Clear();
        spawnedPosition.Clear();

        int spawnCount = _CreateCount;

        // �Ϻθ� ������ Ȯ�� ����
        if (Random.value > _spawnCountChance)
        {
            spawnCount = Random.Range((int)(_CreateCount * 0.5f), _CreateCount);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnNewObject();
        }
    }


    // Ȯ���� ���� ���� ������ ����
    protected virtual T GetRandomSpawnData()
    {
        float totalWeight = 0f;
        foreach (var data in _spawnDataList)
        {
            totalWeight += data._spawnProbability;
        }

        float rand = Random.Range(0f, totalWeight);
        float sum = 0f;

        foreach (var data in _spawnDataList)
        {
            sum += data._spawnProbability;
            if (rand <= sum)
            {
                return data;
            }
        }

        return _spawnDataList[0]; // Ǯ��
    }

    // ���� ������Ʈ ����
    protected virtual void SpawnNewObject()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            T spawnData = GetRandomSpawnData();

            if (spawnData.mapPrefab == null) continue;

            // ���� XZ ��ǥ ����
            float x = Random.Range(-_mapSwpanSize.x / 2, _mapSwpanSize.x / 2);
            float z = Random.Range(-_mapSwpanSize.y / 2, _mapSwpanSize.y / 2);
            Vector3 spawnPos = new Vector3(x, _raycastHeight, z);

            // ���� ���� Ž��
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
            {
                Vector3 groundPos = hit.point;

                // �ʹ� ����� ��ġ ����
                bool tooClose = false;
                foreach (var pos in spawnedPosition)
                {
                    if (Vector3.Distance(groundPos, pos) < _minSpawnDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose) continue;

                // ������Ʈ ����
                GameObject instance = Instantiate(spawnData.mapPrefab, groundPos, Quaternion.identity);
                spawnedPosition.Add(groundPos);
                spawnedObjects.Add(instance, spawnData);

                // �ı� ���� Ʈ��Ŀ �߰�
                if (enableRespawn)
                {
                    var tracker = instance.AddComponent<DestructionTracker>();
                    tracker.Initialize(this);
                }

                // ���� �����ʿ��� �ʿ��� �߰� ó��
                OnObjectSpawnered(instance, spawnData);

                return;
            }
        }

        Debug.LogWarning($"{typeof(T).Name} ���� ����: ��ȿ�� ��ġ�� ã�� ����");
    }

    // ���� �����ʿ��� �߰� �ʱ�ȭ ó�� (��: ���� �ʱ�ȭ)
    protected abstract void OnObjectSpawnered(GameObject instance, T spawnData);

    // ������Ʈ �ı� �� ȣ�� > ���� �ð� �� �����
    private void NotifyObjectDestroyed(GameObject obj)
    {
        if (!enableRespawn || !spawnedObjects.ContainsKey(obj)) return;

        T data = spawnedObjects[obj];
        spawnedObjects.Remove(obj);

        StartCoroutine(RespawnAfterDelay(data, respawnDelay));
    }

    // ������ �� �����
    private IEnumerator RespawnAfterDelay(T data, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNewObject();
    }

    void IRespawnNotifier.NotifyObjectDestroyed(GameObject obj)
    {
        NotifyObjectDestroyed(obj);
    }
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
        if (spawner != null)
        {
            spawner.NotifyObjectDestroyed(gameObject);
        }
    }
}
