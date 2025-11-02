using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상자/컨테이너 시스템
/// </summary>
public class Chest : MonoBehaviour
{
    [Header("Chest Settings")]
    [SerializeField] private int chestSize = 9;
    
    [Header("UI References")]
    [SerializeField] private GameObject chestRoot;
    [SerializeField] private Transform slotParent;
    
    [Header("상자에 들어있는 아이템")]
    [SerializeField] private List<Item> initialItems;

    // 상자의 아이템들
    public List<Item> items;
    
    // UI 슬롯들
    private ChestSlot[] slots;

    // 플레이어 인벤토리 참조
    private Inventory playerInventory;

    private void Awake()
    {
        InitializeChest();
        HideChest();
    }

    private void Start()
    {
        // 플레이어 인벤토리 찾기
        if (playerInventory == null)
            playerInventory = FindObjectOfType<Inventory>();
    }

    private void InitializeChest()
    {
        items = new List<Item>();
        for (int i = 0; i < chestSize; i++)
        {
            items.Add(null);
        }
        
        // 초기 아이템 설정
        if (initialItems != null && initialItems.Count > 0)
        {
            for (int i = 0; i < initialItems.Count && i < chestSize; i++)
            {
                items[i] = initialItems[i];
            }
        }
        
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        if (slotParent == null)
        {
            Debug.LogWarning("Chest: slotParent가 설정되지 않았습니다.");
            return;
        }

        slots = slotParent.GetComponentsInChildren<ChestSlot>();
        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning("Chest: 슬롯을 찾을 수 없습니다.");
            return;
        }

        // 슬롯에 아이템 할당
        for (int i = 0; i < slots.Length && i < items.Count; i++)
        {
            slots[i].Item = items[i];
        }
    }

    /// <summary>
    /// 상자 열림 여부를 반환합니다.
    /// </summary>
    public bool IsOpen => chestRoot != null && chestRoot.activeSelf;

    /// <summary>
    /// 상자 열기/닫기를 토글합니다.
    /// </summary>
    public void ToggleChest()
    {
        if (IsOpen)
            HideChest();
        else
            ShowChest();
    }

    /// <summary>
    /// 상자를 표시합니다.
    /// </summary>
    public void ShowChest()
    {
        if (chestRoot != null)
        {
            chestRoot.SetActive(true);
            RefreshSlots();
        }
    }

    /// <summary>
    /// 상자를 숨깁니다.
    /// </summary>
    public void HideChest()
    {
        if (chestRoot != null)
            chestRoot.SetActive(false);
    }

    /// <summary>
    /// 상자 슬롯을 새로고침합니다.
    /// </summary>
    public void RefreshSlots()
    {
        if (slots == null || items == null)
        {
            InitializeSlots();
            return;
        }

        for (int i = 0; i < slots.Length && i < items.Count; i++)
        {
            slots[i].Item = items[i];
        }
    }

    /// <summary>
    /// 아이템을 상자에서 가져가기
    /// </summary>
    /// <param name="item">가져갈 아이템</param>
    /// <param name="quantity">가져갈 수량</param>
    public void TakeItem(Item item, int quantity = 1)
    {
        if (item == null || playerInventory == null)
        {
            Debug.LogWarning("Chest: 아이템 또는 인벤토리를 찾을 수 없습니다.");
            return;
        }

        // 인벤토리에 아이템 추가
        bool added = false;
        for (int i = 0; i < quantity; i++)
        {
            if (playerInventory.AddItem(item))
            {
                added = true;
                // 상자에서 제거
                int idx = items.FindIndex(x => x == item);
                if (idx != -1)
                {
                    items[idx] = null;
                }
            }
        }

        if (added)
        {
            RefreshSlots();
            Debug.Log($"'{item.itemName}' 아이템을 상자에서 가져왔습니다.");
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다.");
        }
    }
}

