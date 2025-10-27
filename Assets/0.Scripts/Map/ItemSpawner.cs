using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ItemSpawnData
{
    public GameObject mapItemPrefab;
    [Range(0f, 1f)] public float _spawnProbability = 1f;
}

public class ItemSpawner : MonoBehaviour
{
    private float _raycastHeight = 100;

    //���������� ������ ��ųʸ� (key = prefab.tag)
    private Dictionary<string, GameObject> _spawnItemPrefab = new Dictionary<string, GameObject>();

    //���� ��ġ ��ħ ������
    private List<Vector3> spawnedPosition = new List<Vector3>();

    //���� ����
    [SerializeField] private List<ItemSpawnData> _spawnItemPrefabList = new List<ItemSpawnData>();
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _CreateItemCount;
    [SerializeField] private Vector2 _mapSwpanSize;
    [SerializeField] private float _minItemDistance;
    [Range(0f, 1f)][SerializeField] private float _spawnCountChance = 1f;


    private void Awake()
    {
        UpdateDictionatyFromList();
    }


    private void Start()
    {
        SpawnItem();
    }


    private void OnValidate()
    {
        UpdateDictionatyFromList();
    }


    //������ ����Ʈ�� ��ųʸ��� ��ȯ
    private void UpdateDictionatyFromList()
    {
        _spawnItemPrefab.Clear();

        foreach (var item in _spawnItemPrefabList)
        {
            if (item.mapItemPrefab == null)
            {
                continue;
            }

            string key = item.mapItemPrefab.tag;

            if (string.IsNullOrEmpty(key) || key == "Untagged")
            {
                continue;
            }

            if (!_spawnItemPrefab.ContainsKey(key))
            {
                _spawnItemPrefab.Add(key, item.mapItemPrefab);
            }
        }
    }


    //������ ���� Ȯ��
    private ItemSpawnData ItemRandomProbability()
    {
        float totalWeight = 0f;
        foreach (var item in _spawnItemPrefabList)
        {
            totalWeight += item._spawnProbability;
        }
        float rand = Random.Range(0, totalWeight);
        float sum = 0f;

        foreach (var item in _spawnItemPrefabList)
        {
            sum += item._spawnProbability;
            if (rand <= sum)
            {
                return item;
            }
        }
        return _spawnItemPrefabList[0]; //Ǯ��
    }

    //������ ���� ����
    private void SpawnItem()
    {
        spawnedPosition.Clear();
        int itemCount = _CreateItemCount;

        if (Random.value > _spawnCountChance)
        {
            itemCount = Random.Range((int)(_CreateItemCount * 0.5f), _CreateItemCount);
        }


        for (int i = 0; i < _CreateItemCount; i++)
        {
            bool placed = false;

            //100�� �ݺ�
            for (int j = 0; j < 100; j++)
            {
                var itemData = ItemRandomProbability();
                GameObject prefab = itemData.mapItemPrefab;

                //���� XZ ��ǥ
                float x = Random.Range(-_mapSwpanSize.x / 2, _mapSwpanSize.x / 2);
                float z = Random.Range(-_mapSwpanSize.y / 2, _mapSwpanSize.y / 2);
                Vector3 spwanPos = new Vector3(x, _raycastHeight, z);

                //���� ���� ���߱�
                if (Physics.Raycast(spwanPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
                {
                    Vector3 groundPos = hit.point;

                    //�ʹ� ������� (��ħ����)
                    bool tooClose = false;
                    foreach (var pos in spawnedPosition)
                    {
                        if (Vector3.Distance(groundPos, pos) < _minItemDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (tooClose)
                    {
                        continue;
                    }

                    //������ ����
                    Instantiate(prefab, groundPos, Quaternion.identity);
                    spawnedPosition.Add(groundPos);

                    placed = true;
                    break;
                }
            }

            if (placed == false)
            {
                Debug.Log($"{i + 1}��° �������� ��ġ�� ��ġ�� ã�� ���߽��ϴ�");
            }
        }
    }
}

