using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ������ ������ �⺻ ���� (������ + Ȯ��)
[System.Serializable]
public class SpawnData
{
    public GameObject mapPrefab;
    [Range(0f, 1f)] public float _spawnProbability = 1f;
}


// ����� �±� ������
public enum TagType
{
    Monster,
    Item,
    ItemBox
}


// �±׺� ���� ���� ����ü
[System.Serializable]
public class TagSpawnSetting
{
    public bool enable = true;
    public List<SpawnData> spawnPrefabs = new List<SpawnData>();
    public TagType tag;
    public int createCount;
    public float minSpawnDistance;
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
        if (spawner != null)
            spawner.NotifyObjectDestroyed(gameObject);
    }
}


// ���� ������
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


    private float _raycastHeight = 100f;

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
            if (tagGroup.enable)
                SpawnTagGroup(tagGroup);
        }
    }


    //�±� �׷캰 ����
    private void SpawnTagGroup(TagSpawnSetting group)
    {
        if (group.spawnPrefabs.Count == 0)
            return;

        int spawnCount = group.createCount;

        //�Ϻθ� ������ Ȯ�� ����
        if (Random.value > _spawnCountChance)
            spawnCount = Random.Range((int)(group.createCount * 0.5f), group.createCount);

        for (int i = 0; i < spawnCount; i++)
            SpawnObjectByTag(group);
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
                return data;
        }
        return list[0];
    }


    //���� ������Ʈ ����
    private void SpawnObjectByTag(TagSpawnSetting group)
    {
        for (int i = 0; i < 100; i++)
        {
            var spawnData = GetRandomData(group.spawnPrefabs);
            if (spawnData == null || spawnData.mapPrefab == null)
            {
                continue;
            }

            //���� ��ġ ����
            float x = Random.Range(-_mapSpawnSize.x / 2, _mapSpawnSize.x / 2);
            float z = Random.Range(-_mapSpawnSize.y / 2, _mapSpawnSize.y / 2);
            Vector3 spawnPos = new Vector3(x, _raycastHeight, z);


            //���� ���� Ž��
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
            {
                Vector3 groundPos = hit.point;


                //�ʹ� ����� ��ġ ����
                bool tooClose = false;
                foreach (var pos in _spawnedPositions)
                {
                    if (Vector3.Distance(groundPos, pos) < group.minSpawnDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose)
                {
                    continue;
                }


                //������Ʈ ���� �� �±� ����
                GameObject instance = Instantiate(spawnData.mapPrefab, groundPos, Quaternion.identity);
                instance.tag = group.tag.ToString();

                _spawnedPositions.Add(groundPos);
                _spawnedObjects.Add(instance, group);


                // �ı� ���� Ʈ��Ŀ �߰�
                if (enableRespawn)
                {
                    var tracker = instance.AddComponent<DestructionTracker>();
                    tracker.Initialize(this);
                }
                return;
            }
        }
        Debug.Log($"[{group.tag}] ���� ����: ��ȿ�� ��ġ�� ã�� ����");
    }


    // ������Ʈ �ı� �� ������ ó��
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


    // ������ �� ������
    private IEnumerator RespawnAfterDelay(TagSpawnSetting group, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnObjectByTag(group);
    }
}
