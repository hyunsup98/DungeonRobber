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

    //내부적으로 관리할 딕셔너리 (key = prefab.tag)
    private Dictionary<string, GameObject> _spawnItemPrefab = new Dictionary<string, GameObject>();

    //스폰 위치 겹침 방지용
    private List<Vector3> spawnedPosition = new List<Vector3>();

    //스폰 설정
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


    //프리펩 리스트를 딕셔너리로 변환
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


    //아이템 랜덤 확률
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
        return _spawnItemPrefabList[0]; //풀백
    }

    //아이템 랜덤 스폰
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

            //100번 반복
            for (int j = 0; j < 100; j++)
            {
                var itemData = ItemRandomProbability();
                GameObject prefab = itemData.mapItemPrefab;

                //랜덤 XZ 좌표
                float x = Random.Range(-_mapSwpanSize.x / 2, _mapSwpanSize.x / 2);
                float z = Random.Range(-_mapSwpanSize.y / 2, _mapSwpanSize.y / 2);
                Vector3 spwanPos = new Vector3(x, _raycastHeight, z);

                //지면 높이 맞추기
                if (Physics.Raycast(spwanPos, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
                {
                    Vector3 groundPos = hit.point;

                    //너무 가까울경우 (겹침방지)
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

                    //아이템 생성
                    Instantiate(prefab, groundPos, Quaternion.identity);
                    spawnedPosition.Add(groundPos);

                    placed = true;
                    break;
                }
            }

            if (placed == false)
            {
                Debug.Log($"{i + 1}번째 아이템을 배치할 위치를 찾지 못했습니다");
            }
        }
    }
}

