using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 아이템 관련 기능을 관리하는 컨트롤러
/// </summary>
public class Item_Controller : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private QuickSlot_Controller quickSlots;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Entity player; // 버프 적용을 위한 플레이어 Entity
    
    [Header("아이템 줍기 설정")]
    [Tooltip("아이템 줍기 레이캐스트 거리")]
    [SerializeField] private float pickupRaycastDistance = 100f;
    [Tooltip("아이템 레이어 마스크 (비워두면 모든 레이어 검사)")]
    [SerializeField] private LayerMask itemLayerMask = ~0; // 기본값: 모든 레이어
    
    private void Awake()
    {
        // 참조가 없으면 자동으로 찾기
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        if (player == null)
            player = FindObjectOfType<Player_Controller>();
    }
    
    /// <summary>
    /// 퀵슬롯 입력 처리 (1~6번 키)
    /// </summary>
    public void HandleQuickSlotInput()
    {
        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (quickSlots != null)
                {
                    Item item = quickSlots.GetItem(i);
                    if (item != null)
                    {
                        UseItem(item, i);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    /// <param name="item">사용할 아이템</param>
    /// <param name="slotIndex">퀵슬롯 인덱스</param>
    public void UseItem(Item item, int slotIndex)
    {
        if (item == null || quickSlots == null)
            return;

        Debug.Log($"아이템 사용 시도: {item.itemName}");

        // 소비형 아이템 처리
        if (item.itemType == Item.ItemType.Consumable)
        {
            // 아이스볼 특별 처리
            if (item.itemName == "아이스볼")
            {
                ThrowFreezeGrenade(item, slotIndex);
                return;
            }

            // 일반 아이템 버프 적용
            if (item.useBuff != null && player != null)
            {
                try
                {
                    // 플레이어의 stats에 버프 적용
                    player.ApplyBuffToEntity(item.useBuff);
                    Debug.Log($"'{item.itemName}' 버프 효과가 적용되었습니다!");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"'{item.itemName}' 버프 적용 중 오류 발생: {e.Message}");
                }
            }
            else
            {
                Debug.Log($"'{item.itemName}' 아이템을 사용했습니다! (버프 효과 없음)");
            }

            // 인벤토리 수량 감소
            if (inventory != null)
            {
                inventory.DecreaseItemQuantity(item);

                // 인벤토리 수량 확인
                uint remainingQuantity = inventory.GetItemQuantity(item);

                if (remainingQuantity == 0)
                {
                    // 수량이 0이 되면 퀵슬롯에서도 제거
                    quickSlots.RemoveItem(slotIndex);
                }
                else
                {
                    // 수량이 남아있으면 퀵슬롯 수량 동기화
                    quickSlots.RefreshSlots();
                }
            }
        }
        else if (item.itemType == Item.ItemType.Equipment)
        {
            // TODO: 장비 장착 로직
            Debug.Log($"'{item.itemName}' 장비를 장착했습니다!");
        }
        else
        {
            Debug.Log($"'{item.itemName}'는 사용할 수 없는 아이템입니다.");
        }
    }

    /// <summary>
    /// 아이스볼 던지기
    /// </summary>
    private void ThrowFreezeGrenade(Item item, int slotIndex)
    {
        // 마우스 위치로 레이캐스트
        Ray ray = mainCamera != null ? mainCamera.ScreenPointToRay(Input.mousePosition) : Camera.main.ScreenPointToRay(Input.mousePosition);
        UnityEngine.RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            Vector3 throwPosition = hit.point;

            // 범위 내의 적 찾기
            Collider[] colliders = Physics.OverlapSphere(throwPosition, 3f); // 3미터 반경

            bool hitEnemy = false;
            foreach (var col in colliders)
            {
                Monster enemy = col.GetComponent<Monster>();
                if (enemy != null)
                {
                    // 빙결 버프 적용
                    if (item.useBuff != null)
                    {
                        enemy.ApplyBuffToEntity(item.useBuff);
                        hitEnemy = true;
                        Debug.Log($"'{enemy.name}'에게 아이스볼맞았습니다!");
                    }
                }
            }

            if (!hitEnemy)
            {
                Debug.Log("아이스볼이 적에게 맞지 않았습니다.");
            }

            // 인벤토리 수량 감소
            if (inventory != null)
            {
                inventory.DecreaseItemQuantity(item);

                uint remainingQuantity = inventory.GetItemQuantity(item);

                if (remainingQuantity == 0)
                {
                    quickSlots.RemoveItem(slotIndex);
                }
                else
                {
                    quickSlots.RefreshSlots();
                }
            }
        }
    }

    /// <summary>
    /// 아이템 버프 적용 (컨텍스트 메뉴에서 호출)
    /// </summary>
    /// <param name="buff">적용할 버프</param>
    public void ApplyItemBuff(BaseBuff buff)
    {
        if (buff != null && player != null)
        {
            try
            {
                // 플레이어의 stats에 버프 적용
                player.ApplyBuffToEntity(buff);
                Debug.Log($"버프 효과가 적용되었습니다!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"버프 적용 중 오류 발생: {e.Message}");
            }
        }
    }
    
    /// <summary>
    /// 맵에서 아이템 줍기 시도
    /// </summary>
    /// <param name="playerPosition">플레이어 위치</param>
    public void TryPickupItem(Vector3 playerPosition)
    {
        // UI 위에 마우스가 있는지 확인 (UI 클릭은 무시)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        Camera cam = mainCamera != null ? mainCamera : Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Item_Controller: 카메라를 찾을 수 없습니다.");
            return;
        }
        
        // 카메라에서 마우스 위치로 레이 생성
        Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);
        
        // 플레이어 위치에서 마우스가 가리키는 방향으로 레이캐스트
        // 카메라 레이의 방향을 사용하되, 시작점은 플레이어 위치
        Vector3 rayDirection = cameraRay.direction;
        Vector3 rayOrigin = playerPosition + Vector3.up * 0.5f; // 플레이어 위치에서 약간 위에서 시작 (눈 높이)
        
        Ray ray = new Ray(rayOrigin, rayDirection);
        UnityEngine.RaycastHit[] hits = Physics.RaycastAll(ray, pickupRaycastDistance, itemLayerMask);

        // 모든 레이캐스트 결과 중 GroundItem 찾기
        foreach (var hit in hits)
        {
            GroundItem groundItem = hit.collider.GetComponent<GroundItem>();
            
            if (groundItem == null)
            {
                groundItem = hit.collider.GetComponentInParent<GroundItem>();
            }
            
            if (groundItem != null)
            {
                float distance = Vector3.Distance(groundItem.transform.position, playerPosition);
                
                if (groundItem.CanPickup(playerPosition) || distance <= pickupRaycastDistance)
                {
                    Item pickedItem = groundItem.Pickup();
                    
                    if (pickedItem != null && inventory != null)
                    {
                        if (inventory.AddItem(pickedItem))
                        {
                            Debug.Log($"'{pickedItem.itemName}' 아이템을 획득했습니다!");
                        }
                        else
                        {
                            Debug.LogWarning("인벤토리가 가득 찼습니다.");
                            groundItem.gameObject.SetActive(true);
                        }
                    }
                }
                return;
            }
        }
    }
    
    /// <summary>
    /// 플레이어 주변의 아이템을 찾아서 줍기 시도
    /// </summary>
    /// <param name="playerPosition">플레이어 위치</param>
    /// <param name="pickupRange">줍기 가능 거리</param>
    /// <returns>아이템을 줍았는지 여부</returns>
    public bool TryPickupNearbyItem(Vector3 playerPosition, float pickupRange = 5f)
    {
        if (inventory == null)
        {
            Debug.LogWarning("Item_Controller: 인벤토리가 설정되지 않았습니다.");
            return false;
        }
        
        // 주변의 모든 GroundItem 찾기
        GroundItem[] allGroundItems = FindObjectsOfType<GroundItem>();
        
        GroundItem closestItem = null;
        float closestDistance = float.MaxValue;
        
        // 가장 가까운 아이템 찾기
        foreach (GroundItem groundItem in allGroundItems)
        {
            if (groundItem == null || groundItem.item == null || !groundItem.gameObject.activeInHierarchy)
                continue;
            
            float distance = Vector3.Distance(groundItem.transform.position, playerPosition);
            
            if (distance <= pickupRange && distance < closestDistance)
            {
                closestItem = groundItem;
                closestDistance = distance;
            }
        }
        
        // 가장 가까운 아이템 줍기 시도
        if (closestItem != null)
        {
            Item pickedItem = closestItem.Pickup();
            
            if (pickedItem != null)
            {
                if (inventory.AddItem(pickedItem))
                {
                    Debug.Log($"'{pickedItem.itemName}' 아이템을 획득했습니다!");
                    return true;
                }
                else
                {
                    Debug.LogWarning("인벤토리가 가득 찼습니다.");
                    // 아이템 다시 활성화
                    closestItem.gameObject.SetActive(true);
                    return false;
                }
            }
        }
        
        return false;
    }
}
