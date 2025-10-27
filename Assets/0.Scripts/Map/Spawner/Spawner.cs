using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 기본 스폰 데이터 구조 (추상클래스)
[System.Serializable]
public abstract class SpawnData
{
    public GameObject mapPrefab;
    [Range(0f, 1f)] public float _spawnProbability = 1f;
}


// 아이템 데이터
[System.Serializable] public class ItemSpawnData : SpawnData {}


// 몬스터 데이터 
[System.Serializable] public class MonsterData : SpawnData {}



//스폰된 오브젝트가 파괴될 때 스포너에 알림을 주기 위한 인터페이스
public interface IRespawnNotifier
{
    void NotifyObjectDestroyed(GameObject obj);
}


//스포너 (제네릭)
public abstract class Spawner<T> : MonoBehaviour,
IRespawnNotifier
where T : SpawnData
{

    [Header("스폰 설정")]
    [SerializeField] private List<T> _spawnDataList = new List<T>();
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _CreateCount;
    [SerializeField] private Vector2 _mapSwpanSize;
    [SerializeField] private float _minSpawnDistance;
    [Range(0f, 1f)][SerializeField] private float _spawnCountChance = 1f;

    [Header("리스폰 설정")]
    [SerializeField] protected bool enableRespawn = true;
    [SerializeField] protected float respawnDelay = 5f;


    private float _raycastHeight = 100f;

    //스폰 위치 겹침 방지용
    private List<Vector3> spawnedPosition = new List<Vector3>();

    //생성된 오브젝트와 관련 데이터 저장
    protected Dictionary<GameObject, T> spawnedObjects = new Dictionary<GameObject, T>();


    private void Start()
    {
        InitialSpawn();
    }


    // 시작 시 지정 개수만큼 스폰
    protected virtual void InitialSpawn()
    {
        spawnedObjects.Clear();
        spawnedPosition.Clear();

        int spawnCount = _CreateCount;

        // 일부만 생성될 확률 적용
        if (Random.value > _spawnCountChance)
        {
            spawnCount = Random.Range((int)(_CreateCount * 0.5f), _CreateCount);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnNewObject();
        }
    }


    // 확률에 따라 스폰 데이터 선택
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

        return _spawnDataList[0]; // 풀백
    }

    // 실제 오브젝트 생성
    protected virtual void SpawnNewObject()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            T spawnData = GetRandomSpawnData();

            if (spawnData.mapPrefab == null) continue;

            // 랜덤 XZ 좌표 생성
            float x = Random.Range(-_mapSwpanSize.x / 2, _mapSwpanSize.x / 2);
            float z = Random.Range(-_mapSwpanSize.y / 2, _mapSwpanSize.y / 2);
            Vector3 spawnPos = new Vector3(x, _raycastHeight, z);

            // 지면 높이 탐색
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
            {
                Vector3 groundPos = hit.point;

                // 너무 가까운 위치 방지
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

                // 오브젝트 생성
                GameObject instance = Instantiate(spawnData.mapPrefab, groundPos, Quaternion.identity);
                spawnedPosition.Add(groundPos);
                spawnedObjects.Add(instance, spawnData);

                // 파괴 감시 트래커 추가
                if (enableRespawn)
                {
                    var tracker = instance.AddComponent<DestructionTracker>();
                    tracker.Initialize(this);
                }

                // 개별 스포너에서 필요한 추가 처리
                OnObjectSpawnered(instance, spawnData);

                return;
            }
        }

        Debug.LogWarning($"{typeof(T).Name} 스폰 실패: 유효한 위치를 찾지 못함");
    }

    // 개별 스포너에서 추가 초기화 처리 (예: 몬스터 초기화)
    protected abstract void OnObjectSpawnered(GameObject instance, T spawnData);

    // 오브젝트 파괴 시 호출 > 일정 시간 후 재생성
    private void NotifyObjectDestroyed(GameObject obj)
    {
        if (!enableRespawn || !spawnedObjects.ContainsKey(obj)) return;

        T data = spawnedObjects[obj];
        spawnedObjects.Remove(obj);

        StartCoroutine(RespawnAfterDelay(data, respawnDelay));
    }

    // 딜레이 후 재생성
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
        if (spawner != null)
        {
            spawner.NotifyObjectDestroyed(gameObject);
        }
    }
}
