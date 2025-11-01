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
    public List<SpawnData> spawnPrefabs = new List<SpawnData>();
    public TagType tag;
    public int createCount;
    public float minSpawnDistance;

    public float spawnHeight = 0f;  // 추가: 스폰 높이
}


//통합 스포너
public class Spawner : MonoBehaviour, IRespawnNotifier
{

    [Header("스폰 설정")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector2 _mapSpawnSize;
    [Range(0f, 1f)][SerializeField] private float _spawnCountChance = 1f;


    [Header("리스폰 설정")]
    [SerializeField] private bool enableRespawn;
    [SerializeField] private float respawnDelay;


    [Header("태그별 스폰 설정")]
    [SerializeField] private List<TagSpawnSetting> _tagGroups = new List<TagSpawnSetting>();

    private float _raycastHeight = 100;

    //스폰 위치 겹침 방지용
    private List<Vector3> _spawnedPositions = new List<Vector3>();

    //생성된 오브젝트와 관련 데이터 저장
    private Dictionary<GameObject, TagSpawnSetting> _spawnedObjects = new Dictionary<GameObject, TagSpawnSetting>();

    private void Start()
    {
        SpawnAllTags();
    }

    //모든 태그 그룹 스폰
    private void SpawnAllTags()
    {
        foreach (var tagGroup in _tagGroups)
        {
            if (tagGroup.enable == true)
            {
                SpawnTagGroup(tagGroup);
            }
        }
    }

    //태그 그룹별 스폰
    private void SpawnTagGroup(TagSpawnSetting group)
    {
        if (group.spawnPrefabs.Count == 0)
        {
            return;
        }

        int spawnCount = group.createCount;

        //일부만 생성될 확률 적용
        if (Random.value > _spawnCountChance)
        {
            spawnCount = Random.Range((int)(group.createCount * 0.5f), group.createCount);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnObjectByTag(group);
        }
    }


    //확률에 따라 스폰 데이터 선택
    private SpawnData GetRandomData(List<SpawnData> list)
    {
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

    private void SpawnObjectByTag(TagSpawnSetting group, bool useTag = true)
    {
        // 유효한 스폰 위치 후보 리스트
        List<Vector3> candidatePositions = new List<Vector3>();
        int maxAttempts = 100;


        // 랜덤 위치 후보 탐색
        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(-_mapSpawnSize.x / 2, _mapSpawnSize.x / 2);
            float z = Random.Range(-_mapSpawnSize.y / 2, _mapSpawnSize.y / 2);
            Vector3 spawnPos = new Vector3(x, _raycastHeight, z);


            // 지면 탐지
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
            {
                Vector3 groundPos = hit.point;
                bool tooClose = false;


                // 기존 스폰 위치와 최소 거리 체크
                foreach (var pos in _spawnedPositions)
                {
                    if (Vector3.Distance(groundPos, pos) < group.minSpawnDistance)
                    {
                        tooClose = true;
                        break;
                    }
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
            return;
        }


        // 최종 스폰 위치 랜덤 선택
        Vector3 finalPos = candidatePositions[Random.Range(0, candidatePositions.Count)];


        // 스폰 높이 적용
        finalPos.y = group.spawnHeight;


        // 확률 기반으로 스폰 데이터 선택
        var spawnData = GetRandomData(group.spawnPrefabs);
        if (spawnData == null || spawnData.mapPrefab == null)
        {
            return;
        }


        // 오브젝트 생성
        GameObject instance = Instantiate(spawnData.mapPrefab, finalPos, Quaternion.identity);


        // 위치와 객체 데이터 저장
        _spawnedPositions.Add(finalPos);
        _spawnedObjects.Add(instance, group);


        // 파괴 감시 트래커 추가 (리스폰 기능용)
        if (enableRespawn == true)
        {
            var tracker = instance.AddComponent<DestructionTracker>();
            tracker.Initialize(this);
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



    //오브젝트 파괴 시 리스폰 처리
    public void NotifyObjectDestroyed(GameObject obj)
    {
        if (enableRespawn == false || _spawnedObjects.ContainsKey(obj) == false)
        {
            return;
        }

        var group = _spawnedObjects[obj];
        _spawnedObjects.Remove(obj);

        StartCoroutine(RespawnAfterDelay(group, respawnDelay));
    }



    //딜레이 후 리스폰
    private IEnumerator RespawnAfterDelay(TagSpawnSetting group, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnObjectByTag(group);
    }
}
