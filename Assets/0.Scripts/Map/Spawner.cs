using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//������ ������ �⺻ ���� (������ + Ȯ��)
[System.Serializable]
public class SpawnData
{
    public GameObject mapPrefab;
    [Range(0f, 1f)] public float _spawnProbability = 1f;
}


//����� �±� ������
public enum TagType
{
    Monster,
    Item,
    ItemBox
}


//�±׺� ���� ���� ����ü
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

    //���� OFF�� �� ����� ���� ���� ����Ʈ
    public List<Transform> fixedSpawnPoints = new List<Transform>();
}


// ������ �˸� �������̽�
public interface IRespawnNotifier
{
    void NotifyObjectDestroyed(GameObject obj);
}


// ������Ʈ �ı� ���� Ʈ��Ŀ
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


// ���� ������
public class Spawner : MonoBehaviour, IRespawnNotifier
{

    [Header("���� ����")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

    [SerializeField] private float _wallCheckRadius = 2f;
    [SerializeField] private Vector2 _mapSpawnSize = new Vector2(50, 50);
    [Range(0f, 1f)][SerializeField] private float _spawnCountChance = 1f;


    [Header("������ ����")]
    [SerializeField] private bool enableRespawn = true;
    [SerializeField] private float respawnDelay = 3f;


    [Header("�±׺� ���� ����")]
    [SerializeField] private List<TagSpawnSetting> _tagGroups = new List<TagSpawnSetting>();


    private float _raycastHeight = 100;


    //������ ������Ʈ�� ���� ������ ����
    private Dictionary<GameObject, (TagSpawnSetting group, Transform spawnPoint)> _spawnedObjects
    = new Dictionary<GameObject, (TagSpawnSetting, Transform)>();


    //���� ��ġ ��ħ ������
    private List<Vector3> _spawnedPositions = new List<Vector3>();


    private void Start()
    {
        SpawnAllTags();
    }


    //��� �±� �׷� ����
    private void SpawnAllTags()
    {
        foreach (var tagGroup in _tagGroups)
        {
            if (tagGroup.enable)
                SpawnTagGroup(tagGroup);
        }
    }


    //�±� �׷캰 ����
    private void SpawnTagGroup(TagSpawnSetting group)
    {
        if (group.spawnPrefabs.Count == 0)
        {
            return;
        }
            
        //���� OFF �� ���� ����Ʈ ��ġ ����
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

        //���� ON �� ���� ���� ����
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


    //Ȯ���� ���� ���� ������ ����
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


    //������Ʈ ����
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


    //���� ��ġ ���
    private Vector3 GetRandomSpawnPosition(TagSpawnSetting group)
    {

        // ��ȿ�� ���� ��ġ �ĺ� ����Ʈ
        List<Vector3> candidatePositions = new List<Vector3>();
        int maxAttempts = 100;


        // ���� ��ġ �ĺ� Ž��
        for (int i = 0; i < maxAttempts; i++)
        {

            float x = Random.Range(-_mapSpawnSize.x / 2, _mapSpawnSize.x / 2);
            float z = Random.Range(-_mapSpawnSize.y / 2, _mapSpawnSize.y / 2);
            Vector3 spawnPos = transform.position + new Vector3(x, _raycastHeight, z);


            //���� Ž��
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
            {

                Vector3 groundPos = hit.point;
                bool tooClose = false;

                //���� ���� ��ġ�� �ּ� �Ÿ� üũ
                foreach (var pos in _spawnedPositions)
                {
                    if (Vector3.Distance(groundPos, pos) < group.minSpawnDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                //�� ���̾� ����
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


        // ��ȿ ��ġ�� ������ ���� ����
        if (candidatePositions.Count == 0)
        {
            return Vector3.zero;
        }
        return candidatePositions[Random.Range(0, candidatePositions.Count)];
    }


    //������Ʈ �ı� �� ������ ó��
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


    //������ �� ������
    private IEnumerator RespawnAfterDelay(TagSpawnSetting group, Transform originPoint, float delay)
    {
        yield return new WaitForSeconds(delay);

        //���� ON �� ���ο� ���� ��ġ���� ������
        if (group.useRandomSpawn)
        {
            Vector3 pos = GetRandomSpawnPosition(group);
            SpawnObjectByTag(group, pos, null);
        }
        else
        {
            //���� OFF �� ���� ���� ����Ʈ���� ������
            if (originPoint != null)
            {
                SpawnObjectByTag(group, originPoint.position, originPoint);
            }
        }
    }


    //������Ʈ�� �ڽĵ���� �±� ����
    private void SetTagRecursively(GameObject obj, string tag)
    {
        obj.tag = tag;
        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, tag);
        }
    }
}
