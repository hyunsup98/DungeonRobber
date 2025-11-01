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
    public List<SpawnData> spawnPrefabs = new List<SpawnData>();
    public TagType tag;
    public int createCount;
    public float minSpawnDistance;

    public float spawnHeight = 0f;  // �߰�: ���� ����
}


//���� ������
public class Spawner : MonoBehaviour, IRespawnNotifier
{

    [Header("���� ����")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector2 _mapSpawnSize;
    [Range(0f, 1f)][SerializeField] private float _spawnCountChance = 1f;


    [Header("������ ����")]
    [SerializeField] private bool enableRespawn;
    [SerializeField] private float respawnDelay;


    [Header("�±׺� ���� ����")]
    [SerializeField] private List<TagSpawnSetting> _tagGroups = new List<TagSpawnSetting>();

    private float _raycastHeight = 100;

    //���� ��ġ ��ħ ������
    private List<Vector3> _spawnedPositions = new List<Vector3>();

    //������ ������Ʈ�� ���� ������ ����
    private Dictionary<GameObject, TagSpawnSetting> _spawnedObjects = new Dictionary<GameObject, TagSpawnSetting>();

    private void Start()
    {
        SpawnAllTags();
    }

    //��� �±� �׷� ����
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

    //�±� �׷캰 ����
    private void SpawnTagGroup(TagSpawnSetting group)
    {
        if (group.spawnPrefabs.Count == 0)
        {
            return;
        }

        int spawnCount = group.createCount;

        //�Ϻθ� ������ Ȯ�� ����
        if (Random.value > _spawnCountChance)
        {
            spawnCount = Random.Range((int)(group.createCount * 0.5f), group.createCount);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnObjectByTag(group);
        }
    }


    //Ȯ���� ���� ���� ������ ����
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
        // ��ȿ�� ���� ��ġ �ĺ� ����Ʈ
        List<Vector3> candidatePositions = new List<Vector3>();
        int maxAttempts = 100;


        // ���� ��ġ �ĺ� Ž��
        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(-_mapSpawnSize.x / 2, _mapSpawnSize.x / 2);
            float z = Random.Range(-_mapSpawnSize.y / 2, _mapSpawnSize.y / 2);
            Vector3 spawnPos = new Vector3(x, _raycastHeight, z);


            // ���� Ž��
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
            {
                Vector3 groundPos = hit.point;
                bool tooClose = false;


                // ���� ���� ��ġ�� �ּ� �Ÿ� üũ
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


        // ��ȿ ��ġ�� ������ ���� ����
        if (candidatePositions.Count == 0)
        {
            return;
        }


        // ���� ���� ��ġ ���� ����
        Vector3 finalPos = candidatePositions[Random.Range(0, candidatePositions.Count)];


        // ���� ���� ����
        finalPos.y = group.spawnHeight;


        // Ȯ�� ������� ���� ������ ����
        var spawnData = GetRandomData(group.spawnPrefabs);
        if (spawnData == null || spawnData.mapPrefab == null)
        {
            return;
        }


        // ������Ʈ ����
        GameObject instance = Instantiate(spawnData.mapPrefab, finalPos, Quaternion.identity);


        // ��ġ�� ��ü ������ ����
        _spawnedPositions.Add(finalPos);
        _spawnedObjects.Add(instance, group);


        // �ı� ���� Ʈ��Ŀ �߰� (������ ��ɿ�)
        if (enableRespawn == true)
        {
            var tracker = instance.AddComponent<DestructionTracker>();
            tracker.Initialize(this);
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



    //������Ʈ �ı� �� ������ ó��
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



    //������ �� ������
    private IEnumerator RespawnAfterDelay(TagSpawnSetting group, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnObjectByTag(group);
    }
}
