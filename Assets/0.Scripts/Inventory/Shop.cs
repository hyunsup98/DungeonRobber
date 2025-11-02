using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 시스템
/// </summary>
public class Shop : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private int shopSize = 20;
    
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
        
        // 사용 가능한 아이템을 상점 아이템으로 변환
        if (availableItems != null && availableItems.Count > 0)
        {
            foreach (var item in availableItems)
            {
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
    public void TryBuyItem(Item item, int quantity = 1)
    {
        if (item == null || playerInventory == null)
        {
            Debug.LogWarning("Shop: 아이템 또는 인벤토리를 찾을 수 없습니다.");
            return;
        }

        // 구매 가능 여부 확인
        ShopItem shopItem = shopItems.Find(si => si.item == item);
        if (shopItem == null)
        {
            Debug.LogWarning($"Shop: '{item.itemName}'는 판매하지 않습니다.");
            return;
        }

        // 재고 확인 (stock이 -1이면 무제한)
        if (shopItem.stock >= 0 && shopItem.stock < quantity)
        {
            Debug.LogWarning($"Shop: '{item.itemName}'의 재고가 부족합니다. (보유: {shopItem.stock}, 요청: {quantity})");
            return;
        }

        // TODO: 골드 확인 및 차감
        // 여기서는 일단 구매 성공으로 간주

        // 인벤토리에 아이템 추가
        bool added = false;
        for (int i = 0; i < quantity; i++)
        {
            if (playerInventory.AddItem(item))
            {
                added = true;
                // 재고 차감
                if (shopItem.stock > 0)
                {
                    shopItem.stock--;
                }
            }
        }

        if (added)
        {
            Debug.Log($"'{item.itemName}' 아이템을 {quantity}개 구매했습니다.");
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다.");
        }
    }

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

        // 인벤토리에서 아이템 제거
        for (int i = 0; i < quantity; i++)
        {
            playerInventory.RemoveItem(item);
        }

        // TODO: 골드 지급

        Debug.Log($"'{item.itemName}' 아이템을 {quantity}개 판매했습니다. (+{item.sellPrice * quantity}G)");
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

