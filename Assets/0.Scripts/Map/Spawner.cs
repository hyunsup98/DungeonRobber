using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//스폰할 데이터 기본 구조 (프리팹 + 확률)
[System.Serializable]
public class SpawnData
{
    public GameObject mapPrefab;
    [Range(0f, 1f)] public float _spawnProbability = 1f;
}


//사용할 태그 열거형
public enum TagType
{
    Monster,
    Item,
    ItemBox
}


//태그별 스폰 설정 구조체
[System.Serializable]
public class TagSpawnSetting
{
    public bool enable = true;
    public bool useRandomSpawn = true;

    public List<SpawnData> spawnPrefabs = new List<SpawnData>();
    public TagType tag;
    public int createCount;
    public float minSpawnDistance;
    public float spawnHeight = 0f;

    //랜덤 OFF일 때 사용할 고정 스폰 포인트
    public List<Transform> fixedSpawnPoints = new List<Transform>();
}


// 리스폰 알림 인터페이스
public interface IRespawnNotifier
{
    void NotifyObjectDestroyed(GameObject obj);
}


// 오브젝트 파괴 감지 트래커
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


// 통합 스포너
public class Spawner : MonoBehaviour, IRespawnNotifier
{

    [Header("스폰 설정")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

    [SerializeField] private float _wallCheckRadius = 2f;
    [SerializeField] private Vector2 _mapSpawnSize = new Vector2(50, 50);
    [Range(0f, 1f)][SerializeField] private float _spawnCountChance = 1f;


    [Header("리스폰 설정")]
    [SerializeField] private bool enableRespawn = true;
    [SerializeField] private float respawnDelay = 3f;


    [Header("태그별 스폰 설정")]
    [SerializeField] private List<TagSpawnSetting> _tagGroups = new List<TagSpawnSetting>();


    private float _raycastHeight = 100;


    //생성된 오브젝트와 관련 데이터 저장
    private Dictionary<GameObject, (TagSpawnSetting group, Transform spawnPoint)> _spawnedObjects
    = new Dictionary<GameObject, (TagSpawnSetting, Transform)>();


    //스폰 위치 겹침 방지용
    private List<Vector3> _spawnedPositions = new List<Vector3>();


    private void Start()
    {
        SpawnAllTags();
    }


    //모든 태그 그룹 스폰
    private void SpawnAllTags()
    {
        foreach (var tagGroup in _tagGroups)
        {
            if (tagGroup.enable)
                SpawnTagGroup(tagGroup);
        }
    }


    //태그 그룹별 스폰
    private void SpawnTagGroup(TagSpawnSetting group)
    {
        if (group.spawnPrefabs.Count == 0)
        {
            return;
        }
            
        //랜덤 OFF 시 고정 포인트 위치 기준
        if (!group.useRandomSpawn)
        {
            int count = Mathf.Min(group.fixedSpawnPoints.Count, group.createCount);

            for (int i = 0; i < count; i++)
            {
                Transform point = group.fixedSpawnPoints[i];
                if (point != null)
                {
                    SpawnObjectByTag(group, point.position, point);
                }
            }
            return;
        }

        //랜덤 ON 시 기존 랜덤 로직
        int spawnCount = group.createCount;
        if (group.useRandomSpawn && Random.value > _spawnCountChance)
        {
            spawnCount = Random.Range((int)(group.createCount * 0.5f), group.createCount);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = GetRandomSpawnPosition(group);
            SpawnObjectByTag(group, pos, null);
        }
    }


    //확률에 따라 스폰 데이터 선택
    private SpawnData GetRandomData(TagSpawnSetting group)
    {
        var list = group.spawnPrefabs;
        if (group.useRandomSpawn == false)
        {
            return list[0];
        }

        float totalWeight = 0f;
        foreach (var data in list)
        {
            totalWeight += data._spawnProbability;
        }

        float rand = Random.Range(0f, totalWeight);
        float sum = 0f;

        foreach (var data in list)
        {
            sum += data._spawnProbability;
            if (rand <= sum)
            {
                return data;
            }
        }
        return list[0];
    }


    //오브젝트 생성
    private void SpawnObjectByTag(TagSpawnSetting group, Vector3 spawnPos, Transform originPoint)
    {

        if (spawnPos == Vector3.zero)
        {
            return;
        }

        spawnPos.y = group.spawnHeight;
        var spawnData = GetRandomData(group);

        if (spawnData == null || spawnData.mapPrefab == null)
        {
            return;
        }
            
        GameObject instance = Instantiate(spawnData.mapPrefab, spawnPos, Quaternion.identity);
        _spawnedPositions.Add(spawnPos);
        _spawnedObjects[instance] = (group, originPoint);


        if (enableRespawn)
        {
            var tracker = instance.AddComponent<DestructionTracker>();
            tracker.Initialize(this);
        }
    }


    //랜덤 위치 계산
    private Vector3 GetRandomSpawnPosition(TagSpawnSetting group)
    {

        // 유효한 스폰 위치 후보 리스트
        List<Vector3> candidatePositions = new List<Vector3>();
        int maxAttempts = 100;


        // 랜덤 위치 후보 탐색
        for (int i = 0; i < maxAttempts; i++)
        {

            float x = Random.Range(-_mapSpawnSize.x / 2, _mapSpawnSize.x / 2);
            float z = Random.Range(-_mapSpawnSize.y / 2, _mapSpawnSize.y / 2);
            Vector3 spawnPos = transform.position + new Vector3(x, _raycastHeight, z);


            //지면 탐지
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
            {

                Vector3 groundPos = hit.point;
                bool tooClose = false;

                //기존 스폰 위치와 최소 거리 체크
                foreach (var pos in _spawnedPositions)
                {
                    if (Vector3.Distance(groundPos, pos) < group.minSpawnDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                //벽 레이어 감지
                bool nearWall = Physics.CheckSphere(groundPos, _wallCheckRadius, _wallLayer);
                if (nearWall)
                {
                    continue;
                }

                if (tooClose == false)
                {
                    candidatePositions.Add(groundPos);
                }
            }
        }


        // 유효 위치가 없으면 스폰 실패
        if (candidatePositions.Count == 0)
        {
            return Vector3.zero;
        }
        return candidatePositions[Random.Range(0, candidatePositions.Count)];
    }


    //오브젝트 파괴 시 리스폰 처리
    public void NotifyObjectDestroyed(GameObject obj)
    {
        if (enableRespawn == false || _spawnedObjects.ContainsKey(obj) == false)
        {
            return;
        }

        var (group, originPoint) = _spawnedObjects[obj];
        _spawnedObjects.Remove(obj);
        StartCoroutine(RespawnAfterDelay(group, originPoint, respawnDelay));
    }


    //딜레이 후 리스폰
    private IEnumerator RespawnAfterDelay(TagSpawnSetting group, Transform originPoint, float delay)
    {
        yield return new WaitForSeconds(delay);

        //랜덤 ON 시 새로운 랜덤 위치에서 리스폰
        if (group.useRandomSpawn)
        {
            Vector3 pos = GetRandomSpawnPosition(group);
            SpawnObjectByTag(group, pos, null);
        }
        else
        {
            //랜덤 OFF 시 원래 고정 포인트에서 리스폰
            if (originPoint != null)
            {
                SpawnObjectByTag(group, originPoint.position, originPoint);
            }
        }
    }


    //오브젝트와 자식들까지 태그 지정
    private void SetTagRecursively(GameObject obj, string tag)
    {
        obj.tag = tag;
        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, tag);
        }
    }
}
