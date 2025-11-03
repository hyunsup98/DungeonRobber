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
    ItemBox,
    EscapeArea
}


//태그별 스폰 설정 구조체
[System.Serializable]
public class TagSpawnSetting
{

    public bool enable = true;

    //스폰 후보 프리팹 목록
    public List<SpawnData> spawnPrefabs = new List<SpawnData>();

    public TagType tag;
    public int createCount;
    public float minSpawnDistance;
    public float spawnHeight = 0f;

    //태그별 포인트 스폰 사용 여부
    public bool usePreplacedPoints = false;

    //태그별 포인트 리스트(씬에 배치된 Transform)
    public List<Transform> installedSpawnPoints = new List<Transform>();

    //태그별 리스폰 사용 여부
    public bool enableRespawn = false;

    //태그별 리스폰 지연(초)
    public float respawnDelay = 0f;
}


//통합 스포너
public class Spawner : MonoBehaviour, IRespawnNotifier
{
    [Header("스폰 설정")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector2 _mapSpawnSize;
    [Range(0f, 1f)][SerializeField] private float _spawnCountChance = 1f;


    [Header("태그별 스폰 설정")]
    [SerializeField] private List<TagSpawnSetting> _tagGroups = new List<TagSpawnSetting>();

    private float _raycastHeight = 100;

    //스폰 위치 중복 방지
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
            if (tagGroup != null)
            {
                if (tagGroup.enable == true)
                {
                    SpawnTagGroup(tagGroup);
                }
            }
        }
    }


    //태그 그룹별 스폰
    private void SpawnTagGroup(TagSpawnSetting group)
    {
        if (group.spawnPrefabs == null)
        {
            return;
        }

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


        //포인트 스폰 분기
        if (group.usePreplacedPoints == true)
        {
            InstalledSpawnPoints(group, spawnCount);
        }
        else
        {
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnObjectByTag(group);
            }
        }
    }


    //설치된 스폰 포인트에 생성 (씬에 배치된 Transform 사용)
    private void InstalledSpawnPoints(TagSpawnSetting group, int spawnCount)
    {
        if (group.installedSpawnPoints == null)
        {
            return;
        }

        if (group.installedSpawnPoints.Count == 0)
        {
            return;
        }


        //유효 포인트 인덱스 수집
        List<int> indices = new List<int>();

        for (int i = 0; i < group.installedSpawnPoints.Count; i++)
        {
            if (group.installedSpawnPoints[i] != null)
            {
                indices.Add(i);
            }
        }

        if (indices.Count == 0)
        {
            return;
        }


        //스폰 횟수 보정
        int actual = Mathf.Min(spawnCount, indices.Count);

        for (int n = 0; n < actual; n++)
        {
            int pick = Random.Range(0, indices.Count);
            int pointIndex = indices[pick];
            indices.RemoveAt(pick);

            Transform p = group.installedSpawnPoints[pointIndex];

            if (p == null)
            {
                continue;
            }


            //프리팹 선택
            var spawnData = GetRandomData(group.spawnPrefabs);

            if (spawnData == null)
            {
                continue;
            }

            if (spawnData.mapPrefab == null)
            {
                continue;
            }


            //위치 계산
            Vector3 pos = p.position;
            pos.y = group.spawnHeight;


            //생성
            GameObject instance = Instantiate(spawnData.mapPrefab, pos, Quaternion.identity);


            //기록
            _spawnedPositions.Add(pos);
            _spawnedObjects.Add(instance, group);


            //태그별 리스폰 사용 시 트래커 부착(이미 별도 파일에 존재)
            if (group.enableRespawn == true)
            {
                var tracker = instance.GetComponent<DestructionTracker>();
                if (tracker == null)
                {
                    tracker = instance.AddComponent<DestructionTracker>();
                }
                tracker.Initialize(this);
            }
        }
    }


    //확률 기반 프리팹 선택
    private SpawnData GetRandomData(List<SpawnData> list)
    {
        if (list == null)
        {
            return null;
        }

        if (list.Count == 0)
        {
            return null;
        }

        float total = 0f;

        foreach (var data in list)
        {
            total += data._spawnProbability;
        }

        if (total <= 0f)
        {
            return list[0];
        }

        float rand = Random.Range(0f, total);
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


    //랜덤 영역 스폰
    private void SpawnObjectByTag(TagSpawnSetting group, bool useTag = true)
    {

        List<Vector3> candidatePositions = new List<Vector3>();
        int maxAttempts = 100;


        //랜덤 후보 수집
        for (int i = 0; i < maxAttempts; i++)
        {
            float offsetX = transform.position.x;
            float offsetZ = transform.position.z;

            float x = Random.Range(offsetX - (_mapSpawnSize.x / 2f), offsetX + (_mapSpawnSize.x / 2f));
            float z = Random.Range(offsetZ - (_mapSpawnSize.y / 2f), offsetZ + (_mapSpawnSize.y / 2f));
            Vector3 spawnPos = new Vector3(x, _raycastHeight, z);


            //지면 히트
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
            {
                Vector3 groundPos = hit.point;
                bool tooClose = false;


                //최소 거리 검사
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


        //후보 없음
        if (candidatePositions.Count == 0)
        {
            return;
        }


        //최종 위치
        Vector3 finalPos = candidatePositions[Random.Range(0, candidatePositions.Count)];
        finalPos.y = group.spawnHeight;


        //프리팹 선택
        var spawnData = GetRandomData(group.spawnPrefabs);

        if (spawnData == null)
        {
            return;
        }

        if (spawnData.mapPrefab == null)
        {
            return;
        }


        //생성
        GameObject instance = Instantiate(spawnData.mapPrefab, finalPos, Quaternion.identity);


        //기록
        _spawnedPositions.Add(finalPos);
        _spawnedObjects.Add(instance, group);

        //태그별 리스폰 사용 시 트래커 부착
        if (group.enableRespawn == true)
        {
            var tracker = instance.GetComponent<DestructionTracker>();
            if (tracker == null)
            {
                tracker = instance.AddComponent<DestructionTracker>();
            }
            tracker.Initialize(this);
        }
    }


    //오브젝트와 자식 태그 지정
    private void SetTagRecursively(GameObject obj, string tag)
    {
        obj.tag = tag;

        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, tag);
        }
    }


    //파괴 알림 수신(리스폰 결정)
    public void NotifyObjectDestroyed(GameObject obj)
    {
        if (_spawnedObjects.ContainsKey(obj) == false)
        {
            return;
        }

        var group = _spawnedObjects[obj];
        _spawnedObjects.Remove(obj);

        if (group.enableRespawn == false)
        {
            return;
        }

        StartCoroutine(RespawnAfterDelay(group, group.respawnDelay));
    }


    //딜레이 후 리스폰
    private IEnumerator RespawnAfterDelay(TagSpawnSetting group, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (group.usePreplacedPoints == true)
        {
            InstalledSpawnPoints(group, 1);
        }
        else
        {
            SpawnObjectByTag(group);
        }
    }
}
