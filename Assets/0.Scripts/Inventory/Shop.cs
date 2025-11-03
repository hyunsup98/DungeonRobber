using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 상점 시스템
/// </summary>
public class Shop : MonoBehaviour
{
    // [Header("Shop Settings")]
    // [SerializeField] private int shopSize = 20;
    
    [Header("UI References")]
    [SerializeField] private GameObject shopRoot;
    [SerializeField] private Transform slotParent;
    
    [Header("구매 가능한 아이템 목록")]
    [SerializeField] private List<Item> availableItems;

    // 상점의 아이템들
    private List<ShopItem> shopItems;
    
    // UI 슬롯들
    private ShopSlot[] slots;

    // 플레이어 인벤토리 참조
    private Inventory playerInventory;

    [Header("상점 설정")]
    [Tooltip("Inspector에서 아이템 목록이 비어있으면 기본 아이템들을 자동으로 로드합니다")]
    [SerializeField] private bool autoLoadDefaultItems = true;

    private void Awake()
    {
        InitializeShop();
        HideShop();
    }

    private void Start()
    {
        // 플레이어 인벤토리 찾기
        if (playerInventory == null)
            playerInventory = FindObjectOfType<Inventory>();
    }

    private void InitializeShop()
    {
        shopItems = new List<ShopItem>();
        
        // availableItems가 비어있고 자동 로드가 활성화되어 있으면 기본 아이템 로드
        if ((availableItems == null || availableItems.Count == 0) && autoLoadDefaultItems)
        {
            LoadDefaultShopItems();
        }
        
        // 사용 가능한 아이템을 상점 아이템으로 변환
        if (availableItems != null && availableItems.Count > 0)
        {
            foreach (var item in availableItems)
            {
                if (item == null) continue; // null 체크
                
                ShopItem shopItem = new ShopItem
                {
                    item = item,
                    stock = -1 // -1은 무제한
                };
                shopItems.Add(shopItem);
            }
        }
        
        InitializeSlots();
    }
    
    /// <summary>
    /// 기본 상점 아이템 목록을 로드합니다.
    /// </summary>
    private void LoadDefaultShopItems()
    {
        if (availableItems == null)
        {
            availableItems = new List<Item>();
        }
        
#if UNITY_EDITOR
        // 에디터 모드: 직접 경로로 로드
        string[] defaultItemPaths = {
            "Assets/1.Prefabs/Item/Health Potion.asset",
            "Assets/1.Prefabs/Item/Speed Boost Potion.asset"
        };
        
        foreach (string path in defaultItemPaths)
        {
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);
            if (item != null && !availableItems.Contains(item))
            {
                availableItems.Add(item);
                Debug.Log($"Shop: 기본 아이템 로드 완료 - {item.itemName}");
            }
        }
        
        // 프로젝트 내 모든 Item 에셋 찾기
        string[] guids = AssetDatabase.FindAssets("t:Item");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);
            
            if (item != null && !availableItems.Contains(item))
            {
                // 기본 상점 아이템만 추가 (포션류)
                if (item.itemName.Contains("포션") || item.itemName.Contains("Potion") ||
                    item.itemType == Item.ItemType.Consumable)
                {
                    availableItems.Add(item);
                    Debug.Log($"Shop: 프로젝트에서 아이템 발견 - {item.itemName}");
                }
            }
        }
#endif
    }

    private void InitializeSlots()
    {
        if (slotParent == null)
        {
            Debug.LogWarning("Shop: slotParent가 설정되지 않았습니다.");
            return;
        }

        slots = slotParent.GetComponentsInChildren<ShopSlot>();
        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning("Shop: 슬롯을 찾을 수 없습니다.");
            return;
        }

        // 슬롯에 상점 아이템 할당
        for (int i = 0; i < slots.Length && i < shopItems.Count; i++)
        {
            slots[i].ownerInventory = null;
            slots[i].Item = shopItems[i].item;
        }
    }

    /// <summary>
    /// 상점 열림 여부를 반환합니다.
    /// </summary>
    public bool IsOpen => shopRoot != null && shopRoot.activeSelf;

    /// <summary>
    /// 상점 열기/닫기를 토글합니다.
    /// </summary>
    public void ToggleShop()
    {
        if (IsOpen)
            HideShop();
        else
            ShowShop();
    }

    /// <summary>
    /// 상점을 표시합니다.
    /// </summary>
    public void ShowShop()
    {
        if (shopRoot != null)
        {
            shopRoot.SetActive(true);
            RefreshSlots();
        }
    }

    /// <summary>
    /// 상점을 숨깁니다.
    /// </summary>
    public void HideShop()
    {
        if (shopRoot != null)
            shopRoot.SetActive(false);
    }

    /// <summary>
    /// 상점 슬롯을 새로고침합니다.
    /// </summary>
    public void RefreshSlots()
    {
        if (slots == null || shopItems == null)
        {
            InitializeSlots();
            return;
        }

        for (int i = 0; i < slots.Length && i < shopItems.Count; i++)
        {
            slots[i].Item = shopItems[i].item;
        }
    }

    /// <summary>
    /// 아이템 구매 시도
    /// </summary>
    /// <param name="item">구매할 아이템</param>
    /// <param name="quantity">구매 수량</param>
    /// <returns>구매 성공 여부</returns>
    public bool TryBuyItem(Item item, int quantity = 1)
    {
        if (item == null || playerInventory == null)
        {
            Debug.LogWarning("Shop: 아이템 또는 인벤토리를 찾을 수 없습니다.");
            ShowMessage("구매할 수 없습니다. (시스템 오류)");
            return false;
        }

        // 구매 가능 여부 확인
        ShopItem shopItem = shopItems.Find(si => si.item == item);
        if (shopItem == null)
        {
            Debug.LogWarning($"Shop: '{item.itemName}'는 판매하지 않습니다.");
            ShowMessage($"{item.itemName}은(는) 판매하지 않습니다.");
            return false;
        }

        // 재고 확인 (stock이 -1이면 무제한)
        if (shopItem.stock >= 0 && shopItem.stock < quantity)
        {
            Debug.LogWarning($"Shop: '{item.itemName}'의 재고가 부족합니다. (보유: {shopItem.stock}, 요청: {quantity})");
            ShowMessage($"{item.itemName}의 재고가 부족합니다. (보유: {shopItem.stock}개)");
            return false;
        }

        // 골드 확인
        int totalCost = item.buyPrice * quantity;
        if (playerInventory.Gold < totalCost)
        {
            int shortage = totalCost - playerInventory.Gold;
            Debug.LogWarning($"골드가 부족합니다. (보유: {playerInventory.Gold}G, 필요: {totalCost}G)");
            ShowMessage($"골드가 부족합니다.\n보유: {playerInventory.Gold}G / 필요: {totalCost}G\n부족: {shortage}G");
            return false;
        }

        // 인벤토리 공간 확인
        int availableSlots = 0;
        for (int i = 0; i < playerInventory.items.Count; i++)
        {
            if (playerInventory.items[i] == null)
                availableSlots++;
        }

        if (availableSlots < quantity)
        {
            Debug.LogWarning("인벤토리 공간이 부족합니다.");
            ShowMessage($"인벤토리 공간이 부족합니다.\n필요한 공간: {quantity}개 / 사용 가능: {availableSlots}개");
            return false;
        }

        // 아이템 구매 처리
        int successCount = 0;
        for (int i = 0; i < quantity; i++)
        {
            if (playerInventory.AddItem(item))
            {
                successCount++;
                // 재고 차감
                if (shopItem.stock > 0)
                {
                    shopItem.stock--;
                }
            }
        }

        if (successCount > 0)
        {
            // 골드 차감
            playerInventory.Gold -= totalCost;
            
            // 상점 슬롯 새로고침
            RefreshSlots();
            
            string message = quantity > 1 
                ? $"{item.itemName}을(를) {successCount}개 구매했습니다. (-{totalCost}G)"
                : $"{item.itemName}을(를) 구매했습니다. (-{totalCost}G)";
            
            Debug.Log(message);
            ShowMessage(message, true);
            
            // 구매 성공 이벤트 호출
            OnItemPurchased?.Invoke(item, successCount, totalCost);
            
            return true;
        }
        else
        {
            Debug.LogWarning("아이템 구매에 실패했습니다.");
            ShowMessage("아이템 구매에 실패했습니다.");
            return false;
        }
    }
    
    /// <summary>
    /// 메시지를 표시합니다. (선택적으로 오버라이드 가능)
    /// </summary>
    /// <param name="message">표시할 메시지</param>
    /// <param name="isSuccess">성공 메시지 여부</param>
    protected virtual void ShowMessage(string message, bool isSuccess = false)
    {
        // 기본적으로 Debug.Log로 출력
        // 필요시 UI 시스템과 연동하여 팝업으로 표시 가능
        if (isSuccess)
        {
            Debug.Log($"[상점] {message}");
        }
        else
        {
            Debug.LogWarning($"[상점] {message}");
        }
    }
    
    /// <summary>
    /// 아이템 구매 성공 이벤트
    /// </summary>
    public System.Action<Item, int, int> OnItemPurchased;

    /// <summary>
    /// 플레이어 인벤토리 아이템 판매
    /// </summary>
    /// <param name="item">판매할 아이템</param>
    /// <param name="quantity">판매 수량</param>
    public void SellItem(Item item, int quantity = 1)
    {
        if (item == null || playerInventory == null)
        {
            Debug.LogWarning("Shop: 아이템 또는 인벤토리를 찾을 수 없습니다.");
            return;
        }

        // 인벤토리에서 아이템 수량 감소
        for (int i = 0; i < quantity; i++)
        {
            playerInventory.DecreaseItemQuantity(item);
        }

        // 골드 지급
        int sellPrice = item.sellPrice * quantity;
        playerInventory.Gold += sellPrice;

        Debug.Log($"'{item.itemName}' 아이템을 {quantity}개 판매했습니다. (+{sellPrice}G)");
    }

    /// <summary>
    /// 상점 아이템 데이터 구조
    /// </summary>
    [System.Serializable]
    public class ShopItem
    {
        public Item item;
        public int stock = -1; // -1은 무제한
    }
}

