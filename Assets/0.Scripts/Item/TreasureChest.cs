using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보물 상자 - 상호작용 시 랜덤 아이템을 뿌리는 상자
/// </summary>
public class TreasureChest : MonoBehaviour
{
    [Header("상자 설정")]
    [Tooltip("상자에서 나올 아이템 개수")]
    [SerializeField] private int itemCount = 3;
    
    [Tooltip("아이템이 튀어나가는 간격 (초)")]
    [SerializeField] private float spawnInterval = 0.5f;
    
    [Tooltip("아이템이 튀어나가는 힘")]
    [SerializeField] private float ejectForce = 5f;
    
    [Tooltip("아이템이 튀어나가는 높이")]
    [SerializeField] private float ejectHeight = 2f;
    
    [Tooltip("상호작용 가능 거리")]
    [SerializeField] private float interactionRange = 3f;
    
    [Header("아이템 풀")]
    [Tooltip("상자에서 나올 수 있는 아이템 목록 (비어있으면 자동으로 Resources/Items에서 로드)")]
    [SerializeField] private List<Item> itemPool;
    
    [Header("GroundItem 프리팹")]
    [Tooltip("GroundItem 프리팹 (없으면 자동 생성)")]
    [SerializeField] private GameObject groundItemPrefab;
    
    [Header("상태")]
    [SerializeField] private bool isOpened = false;
    [SerializeField] private bool isSpawning = false;
    
    private void Awake()
    {
        // 아이템 풀이 비어있으면 자동으로 로드
        if (itemPool == null || itemPool.Count == 0)
        {
            LoadItemsFromResources();
        }
        
        // GroundItem 프리팹이 없으면 기본 생성
        if (groundItemPrefab == null)
        {
            CreateDefaultGroundItemPrefab();
        }
    }
    
    /// <summary>
    /// Resources/Items 폴더에서 아이템을 자동으로 로드합니다.
    /// </summary>
    private void LoadItemsFromResources()
    {
        itemPool = new List<Item>();
        
        // Resources/Items 폴더에서 모든 Item ScriptableObject 로드
        Item[] loadedItems = Resources.LoadAll<Item>("Items");
        
        if (loadedItems != null && loadedItems.Length > 0)
        {
            itemPool.AddRange(loadedItems);
            Debug.Log($"TreasureChest: Resources에서 {loadedItems.Length}개의 아이템을 로드했습니다.");
        }
        else
        {
            Debug.LogWarning("TreasureChest: Resources/Items 폴더에서 아이템을 찾을 수 없습니다.");
        }
    }
    
    /// <summary>
    /// 기본 GroundItem 프리팹을 생성합니다.
    /// </summary>
    private void CreateDefaultGroundItemPrefab()
    {
        // 간단한 큐브로 GroundItem 생성
        GameObject prefab = new GameObject("GroundItem_Prefab");
        GroundItem groundItem = prefab.AddComponent<GroundItem>();
        
        // 간단한 메시 추가 (큐브)
        GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mesh.transform.SetParent(prefab.transform);
        mesh.transform.localPosition = Vector3.zero;
        mesh.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        // 콜라이더는 유지 (레이캐스트로 줍기 위해 필요)
        Collider collider = mesh.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = false; // 물리 상호작용을 위해 false
        }
        
        // 빛나는 Material 설정
        SetupGlowingMaterial(mesh);
        
        // 작은 빛 추가
        SetupItemLight(prefab);
        
        // Rigidbody 추가 (튀어나가기 위해)
        Rigidbody rb = prefab.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.drag = 2f; // 공기 저항
        rb.mass = 0.1f; // 가벼운 질량
        
        groundItemPrefab = prefab;
        prefab.SetActive(false); // 프리팹이므로 비활성화
    }
    
    /// <summary>
    /// 빛나는 Material을 설정합니다.
    /// </summary>
    private void SetupGlowingMaterial(GameObject meshObj)
    {
        Renderer renderer = meshObj.GetComponent<Renderer>();
        if (renderer == null)
            return;
        
        // 새로운 Material 생성 또는 기존 Material 복사
        Material mat = new Material(Shader.Find("Standard"));
        mat.EnableKeyword("_EMISSION");
        
        // Emission 색상 설정 (아이템이 빛나게)
        Color emissionColor = new Color(1f, 0.8f, 0.3f, 1f); // 황금색 빛
        mat.SetColor("_EmissionColor", emissionColor);
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        
        renderer.material = mat;
    }
    
    /// <summary>
    /// 아이템에 작은 빛을 추가합니다.
    /// </summary>
    private void SetupItemLight(GameObject itemObj)
    {
        Light light = itemObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.8f, 0.3f, 1f); // 황금색
        light.range = 3f; // 작은 범위
        light.intensity = 1.5f;
        light.shadows = LightShadows.None; // 성능을 위해 그림자 없음
    }
    
    /// <summary>
    /// 상자를 엽니다.
    /// </summary>
    public void Open()
    {
        if (isOpened || isSpawning)
            return;
        
        if (itemPool == null || itemPool.Count == 0)
        {
            Debug.LogWarning("TreasureChest: 아이템 풀이 비어있습니다.");
            return;
        }
        
        isOpened = true;
        isSpawning = true;
        
        StartCoroutine(SpawnItemsCoroutine());
    }
    
    /// <summary>
    /// 아이템을 생성하는 코루틴
    /// </summary>
    private IEnumerator SpawnItemsCoroutine()
    {
        // 랜덤 아이템 선택
        List<Item> selectedItems = GetRandomItems(itemCount);
        
        for (int i = 0; i < selectedItems.Count; i++)
        {
            Item item = selectedItems[i];
            if (item == null)
                continue;
            
            // GroundItem 생성 (상자 위치의 약간 위에서)
            Vector3 spawnPosition = transform.position + Vector3.up * 2f;
            GameObject groundItemObj = Instantiate(groundItemPrefab, spawnPosition, Quaternion.identity);
            groundItemObj.SetActive(true);
            
            // GroundItem 컴포넌트 설정
            GroundItem groundItem = groundItemObj.GetComponent<GroundItem>();
            if (groundItem != null)
            {
                groundItem.item = item;
            }
            else
            {
                Debug.LogWarning("TreasureChest: GroundItem 컴포넌트를 찾을 수 없습니다.");
            }
            
            // 빛나는 효과 확인 및 추가 (프리팹에 없을 경우)
            EnsureGlowingEffect(groundItemObj);
            
            // Rigidbody 확인 및 추가
            EnsureRigidbody(groundItemObj);
            
            // 랜덤 방향으로 튀어나가게 함
            EjectItem(groundItemObj);
            
            // 다음 아이템까지 대기
            if (i < selectedItems.Count - 1)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
        isSpawning = false;
        Debug.Log($"TreasureChest: {selectedItems.Count}개의 아이템을 생성했습니다.");
    }
    
    /// <summary>
    /// 랜덤 아이템을 선택합니다.
    /// </summary>
    private List<Item> GetRandomItems(int count)
    {
        List<Item> selected = new List<Item>();
        
        if (itemPool == null || itemPool.Count == 0)
            return selected;
        
        // 아이템 풀에서 중복 허용하여 랜덤 선택
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, itemPool.Count);
            Item randomItem = itemPool[randomIndex];
            
            if (randomItem != null)
            {
                selected.Add(randomItem);
            }
        }
        
        return selected;
    }
    
    /// <summary>
    /// GroundItem에 빛나는 효과가 있는지 확인하고 없으면 추가합니다.
    /// </summary>
    private void EnsureGlowingEffect(GameObject itemObj)
    {
        // Light 컴포넌트 확인
        Light light = itemObj.GetComponent<Light>();
        if (light == null)
        {
            SetupItemLight(itemObj);
        }
        
        // Material Emission 확인
        Renderer renderer = itemObj.GetComponentInChildren<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            Material mat = renderer.material;
            // Emission이 활성화되어 있는지 확인
            if (!mat.IsKeywordEnabled("_EMISSION"))
            {
                SetupGlowingMaterial(renderer.gameObject);
            }
        }
        else
        {
            // Renderer가 없으면 찾아서 설정
            Renderer childRenderer = itemObj.GetComponentInChildren<Renderer>();
            if (childRenderer != null)
            {
                SetupGlowingMaterial(childRenderer.gameObject);
            }
        }
    }
    
    /// <summary>
    /// GroundItem에 Rigidbody가 있는지 확인하고 없으면 추가합니다.
    /// </summary>
    private void EnsureRigidbody(GameObject itemObj)
    {
        Rigidbody rb = itemObj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = itemObj.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.drag = 2f; // 공기 저항
            rb.mass = 0.1f; // 가벼운 질량
            Debug.Log("TreasureChest: GroundItem에 Rigidbody를 추가했습니다.");
        }
    }
    
    /// <summary>
    /// 아이템을 랜덤 방향으로 튀어나가게 합니다.
    /// </summary>
    private void EjectItem(GameObject itemObj)
    {
        Rigidbody rb = itemObj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("TreasureChest: Rigidbody를 추가할 수 없습니다.");
            return;
        }
        
        // 랜덤 방향 계산 (수평면 기준)
        Vector2 randomDir2D = Random.insideUnitCircle.normalized;
        Vector3 randomDirection = new Vector3(randomDir2D.x, ejectHeight, randomDir2D.y);
        
        // 힘 적용
        rb.AddForce(randomDirection * ejectForce, ForceMode.Impulse);
        
        // 랜덤 회전 추가
        Vector3 randomTorque = new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)
        );
        rb.AddTorque(randomTorque, ForceMode.Impulse);
    }
    
    /// <summary>
    /// 플레이어가 상호작용할 수 있는지 확인
    /// </summary>
    public bool CanInteract(Vector3 playerPosition)
    {
        if (isOpened || isSpawning)
            return false;
        
        float distance = Vector3.Distance(transform.position, playerPosition);
        return distance <= interactionRange;
    }
    
    /// <summary>
    /// 상자 열림 여부
    /// </summary>
    public bool IsOpened => isOpened;
    
    // 디버그용 기즈모 그리기
    private void OnDrawGizmos()
    {
        // 상호작용 범위 표시
        Gizmos.color = isOpened ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

