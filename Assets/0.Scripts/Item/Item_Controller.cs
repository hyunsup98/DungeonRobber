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
            // 빙결 수류탄 특별 처리
            if (item.itemName == "빙결 수류탄")
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
    /// 빙결 수류탄 던지기
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
                        Debug.Log($"'{enemy.name}'에게 빙결 수류탄이 맞았습니다!");
                    }
                }
            }

            if (!hitEnemy)
            {
                Debug.Log("빙결 수류탄이 적에게 맞지 않았습니다.");
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
            // UI 위에 마우스가 있으면 아이템 줍기 무시
            return;
        }
        
        // 카메라에서 마우스 위치로 레이캐스트 (모든 오브젝트 검사)
        Ray ray = mainCamera != null ? mainCamera.ScreenPointToRay(Input.mousePosition) : Camera.main.ScreenPointToRay(Input.mousePosition);
        UnityEngine.RaycastHit[] hits = Physics.RaycastAll(ray, pickupRaycastDistance, itemLayerMask);

        // 모든 레이캐스트 결과 중 GroundItem 찾기
        foreach (var hit in hits)
        {
            // GroundItem 찾기 (자식 오브젝트의 Collider에서 부모로 올라가며 찾기)
            GroundItem groundItem = hit.collider.GetComponent<GroundItem>();
            
            // 부모 오브젝트에서 찾기 (자식 mesh의 Collider를 맞췄을 경우)
            if (groundItem == null)
            {
                groundItem = hit.collider.GetComponentInParent<GroundItem>();
            }
            
            // GroundItem을 찾았으면 처리하고 종료
            if (groundItem != null)
            {
                // 줍기 가능한 거리인지 확인
                float distance = Vector3.Distance(groundItem.transform.position, playerPosition);
                if (groundItem.CanPickup(playerPosition))
                {
                    Item pickedItem = groundItem.Pickup();
                    
                    if (pickedItem != null && inventory != null)
                    {
                        if (inventory.AddItem(pickedItem))
                        {
                            Debug.Log($"'{pickedItem.itemName}' 아이템을 획득했습니다! (거리: {distance:F2}m)");
                        }
                        else
                        {
                            Debug.LogWarning("인벤토리가 가득 찼습니다.");
                            // 아이템 다시 활성화
                            groundItem.gameObject.SetActive(true);
                        }
                    }
                    else if (pickedItem == null)
                    {
                        Debug.LogWarning("아이템 데이터가 없습니다.");
                    }
                }
                // GroundItem을 찾았으면 (거리가 멀어도) 다른 오브젝트는 검사하지 않고 종료
                return;
            }
            // GroundItem이 아니면 다음 오브젝트 검사 계속
        }
        
        // GroundItem을 찾지 못했으면 조용히 무시 (Quad 등 다른 오브젝트는 무시)
    }
}
