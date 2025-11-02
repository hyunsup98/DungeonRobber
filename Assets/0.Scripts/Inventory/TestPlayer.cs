using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("인벤토리")]
    public Inventory inventory;

    [Header("퀵슬롯")]
    public QuickSlot_Controller quickSlots;

    [Header("상점")]
    public Shop shop;

    [Header("상점 NPC 감지")]
    [SerializeField] private float interactionRange = 3f;
    
    private GameObject currentNearbyShop = null;
    
    [Header("버프 시스템")]
    [SerializeField] private BaseStat stats = new BaseStat();
    private BuffManager buffManager = new BuffManager();

    void Awake()
    {
        // 스탯 초기화
        stats.Init();
    }

    void Update()
    {
        // 마우스 우클릭 (아이템 획득)
        if (Input.GetMouseButtonDown(1))
        {
            TryPickupItem();
        }

        if (Input.anyKeyDown)
        {
            // 슬롯 1~6번 (퀵슬롯)
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    // 퀵슬롯 i번째 칸에서 아이템 가져오기
                    Item item = quickSlots?.GetItem(i);
                    
                    if (item != null)
                    {
                        UseItem(item, i);
                    }
                }
            }

            // Tab 키 (인벤토리 열기/닫기)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (inventory != null)
                    inventory.ToggleInventory();
            }

            // Q 키 (상점 열기/닫기)
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (currentNearbyShop != null && shop != null)
                {
                    // 상점 열림/닫힘 토글
                    if (!shop.IsOpen)
                    {
                        shop.ShowShop();
                        if (inventory != null && !inventory.IsOpen)
                            inventory.ShowInventory();
                    }
                    else
                    {
                        shop.HideShop();
                        // 인벤토리는 상점이 닫힐 때만 닫음 (Tab으로도 닫을 수 있음)
                    }
                }
                else if (shop != null && shop.IsOpen)
                {
                    // 상점이 열려있고 NPC 근처가 아니면 상점만 닫기
                    shop.HideShop();
                }
            }
        }

        // 상점 NPC 감지
        CheckForNearbyShop();
    }

    /// <summary>
    /// 근처에 상점 NPC가 있는지 확인
    /// </summary>
    private void CheckForNearbyShop()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
        
        GameObject foundShop = null;
        foreach (var col in colliders)
        {
            if (col.CompareTag("Shopper"))
            {
                foundShop = col.gameObject;
                break;
            }
        }

        if (foundShop != currentNearbyShop)
        {
            currentNearbyShop = foundShop;
            if (foundShop != null)
            {
                Debug.Log("[상점] NPC 접촉: Q키를 눌러 상점을 여십시오.");
            }
            else if (shop != null && shop.IsOpen)
            {
                // NPC에서 멀어지면 상점 자동 닫기
                shop.HideShop();
                if (inventory != null && inventory.IsOpen)
                    inventory.HideInventory();
            }
        }
    }

    // 디버그용 기즈모 그리기
    private void OnDrawGizmos()
    {
        Gizmos.color = currentNearbyShop != null ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    /// <summary>
    /// 맵에서 아이템 줍기 시도
    /// </summary>
    private void TryPickupItem()
    {
        // 카메라에서 마우스 위치로 레이캐스트
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            GroundItem groundItem = hit.collider.GetComponent<GroundItem>();
            
            if (groundItem != null)
            {
                // 줍기 가능한 거리인지 확인
                if (groundItem.CanPickup(transform.position))
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
                            // 아이템 다시 활성화
                            groundItem.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    Debug.Log("아이템이 너무 멀리 있습니다.");
                }
            }
        }
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    /// <param name="item">사용할 아이템</param>
    /// <param name="slotIndex">퀵슬롯 인덱스</param>
    private void UseItem(Item item, int slotIndex)
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
            if (item.useBuff != null)
            {
                buffManager.ApplyBuff(item.useBuff, stats);
                Debug.Log($"'{item.itemName}' 버프 효과가 적용되었습니다!");
            }
            else
            {
                // 버프 미구현이어도 일단 로그 확인할 수 있도록 출력
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

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
        if (buff != null)
        {
            buffManager.ApplyBuff(buff, stats);
        }
    }

    //void HitCheckObject(RaycastHit hit3D)
    //{
    //    IObjectItem clickInterface = hit3D.transform.gameObject.GetComponent<IObjectItem>();

    //    if (clickInterface != null)
    //    {
    //        Item item = clickInterface.ClickItem();
    //        bool isAdded = inventory.AddItem(item);
            
    //        // 아이템 획득 효과음
    //        // AudioSource.PlayClipAtPoint(pickupSound, transform.position);

    //        // 아이템 오브젝트 제거
    //        // Destroy 대신 비활성화도 괜찮
    //        //Destroy(hit3D.transform.gameObject);
    //        if(isAdded) hit3D.transform.gameObject.SetActive(false);
    //    }
    //}
}
