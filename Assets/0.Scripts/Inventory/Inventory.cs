using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 인벤토리를 관리하는 메인 클래스
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 9;
    
    [Header("UI References")]
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private Transform slotParent;

    // 아이템 데이터
    public List<Item> items;

    // UI 슬롯들
    private InvenSlot[] slots;

    private void Awake()
    {
        // 인벤토리 크기만큼 null로 초기화
        items = new List<Item>();
        for (int i = 0; i < inventorySize; i++)
        {
            items.Add(null);
        }
        
        InitializeSlots();
        HideInventory();
    }

    private void InitializeSlots()
    {
        if (slotParent == null)
        {
            Debug.LogWarning("Inventory: slotParent가 설정되지 않았습니다.");
            return;
        }

        slots = slotParent.GetComponentsInChildren<InvenSlot>();
        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning("Inventory: 슬롯을 찾을 수 없습니다.");
            return;
        }

        // 슬롯에 인벤토리 참조 할당 및 초기화
        for (int i = 0; i < slots.Length && i < items.Count; i++)
        {
            slots[i].ownerInventory = this;
            slots[i].Item = items[i];
        }
    }

    /// <summary>
    /// 인벤토리 열림 여부를 반환합니다.
    /// </summary>
    public bool IsOpen => inventoryRoot != null && inventoryRoot.activeSelf;

    /// <summary>
    /// 인벤토리 열기/닫기를 토글합니다.
    /// </summary>
    public void ToggleInventory()
    {
        if (IsOpen)
            HideInventory();
        else
            ShowInventory();
    }

    /// <summary>
    /// 인벤토리를 표시합니다.
    /// </summary>
    public void ShowInventory()
    {
        if (inventoryRoot != null)
        {
            inventoryRoot.SetActive(true);
            RefreshSlots();
        }
    }

    /// <summary>
    /// 인벤토리를 숨깁니다.
    /// </summary>
    public void HideInventory()
    {
        // 컨텍스트 메뉴 숨기기
        InventoryContextMenu inventoryContextMenu = InventoryContextMenu.GetOrFind();
        if (inventoryContextMenu != null)
            inventoryContextMenu.Hide();

        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);
    }

    /// <summary>
    /// 인벤토리 슬롯을 새로고침합니다.
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

    #region Item Management

    /// <summary>
    /// 아이템을 인벤토리에 추가합니다.
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    /// <returns>추가 성공 여부</returns>
    public bool AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Inventory: null 아이템은 추가할 수 없습니다.");
            return false;
        }

        if (slots == null || items == null)
        { 
            InitializeSlots();
        }

        // 같은 아이템이 이미 있으면 개수 증가
        if (items.Find(x => x != null && x.name == item.name) != null)
        {
            int itemIdx = items.FindIndex(x => x != null && x.name == item.name);
            Debug.Log($"인벤토리에 이미 '{item.itemName}' 아이템이 있습니다. 개수를 증가시킵니다.");
            slots[itemIdx].ItemQuantity++;
            RefreshSlots();
            return true;
        }

        // 빈 슬롯 찾기
        int idx = items.FindIndex(x => x == null);
        if (idx != -1)
        {
            items[idx] = item;
            slots[idx].ItemQuantity = 1;
            RefreshSlots();
            Debug.Log($"인벤토리에 '{item.itemName}' 아이템이 추가되었습니다.");
            return true;
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다. 아이템을 추가할 수 없습니다.");
            return false;
        }
    }

    /// <summary>
    /// 아이템을 인벤토리에서 제거합니다 (인덱스).
    /// </summary>
    /// <param name="idx">슬롯 인덱스</param>
    public void RemoveItem(int idx)
    {
        if (idx < 0 || idx >= items.Count)
        {
            Debug.LogWarning($"Inventory: 잘못된 인덱스 {idx}");
            return;
        }

            items[idx] = null;
        if (slots != null && idx < slots.Length)
            slots[idx].ItemQuantity = 0;
        
        RefreshSlots();
        Debug.Log($"인벤토리 {idx + 1}번째 아이템이 제거되었습니다.");
    }

    /// <summary>
    /// 아이템을 인벤토리에서 제거합니다 (아이템 오브젝트).
    /// </summary>
    /// <param name="item">제거할 아이템</param>
    public void RemoveItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Inventory: null 아이템은 제거할 수 없습니다.");
            return;
        }

        int idx = items.FindIndex(x => x == item);
        if (idx != -1)
        {
            items[idx] = null;
            if (slots != null && idx < slots.Length)
                slots[idx].ItemQuantity = 0;
            
            RefreshSlots();
            Debug.Log($"인벤토리에서 '{item.itemName}' 아이템이 제거되었습니다.");
        }
        else
        {
            Debug.LogWarning("RemoveItem: 해당 아이템이 인벤토리에 없습니다.");
        }
    }

    /// <summary>
    /// 두 슬롯의 아이템을 교환합니다.
    /// </summary>
    /// <param name="fromIdx">교환할 슬롯 인덱스 1</param>
    /// <param name="toIdx">교환할 슬롯 인덱스 2</param>
    public void SwapItems(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= items.Count || toIdx < 0 || toIdx >= items.Count)
        {
            Debug.LogWarning("Inventory: 잘못된 인덱스입니다.");
            return;
        }

        var tempItem = items[fromIdx];
        var tempQuantity = 0u;
        
        if (slots != null && fromIdx < slots.Length)
        {
            tempQuantity = slots[fromIdx].ItemQuantity;
        }

        items[fromIdx] = items[toIdx];
        items[toIdx] = tempItem;

        if (slots != null)
        {
            if (fromIdx < slots.Length)
                slots[fromIdx].ItemQuantity = toIdx < slots.Length ? slots[toIdx].ItemQuantity : 0;
            if (toIdx < slots.Length)
                slots[toIdx].ItemQuantity = tempQuantity;
        }

        RefreshSlots();
        Debug.Log($"슬롯 {fromIdx + 1}과 {toIdx + 1}의 아이템을 교환했습니다.");
    }

    /// <summary>
    /// 특정 인덱스의 아이템을 반환합니다.
    /// </summary>
    /// <param name="idx">슬롯 인덱스</param>
    /// <returns>아이템 또는 null</returns>
    public Item GetItem(int idx)
    {
        if (idx < 0 || idx >= items.Count)
            return null;
        
        return items[idx];
    }

    /// <summary>
    /// 인벤토리에 빈 공간이 있는지 확인합니다.
    /// </summary>
    /// <returns>빈 공간이 있으면 true</returns>
    public bool HasSpace()
    {
        return items.FindIndex(x => x == null) != -1;
    }

    /// <summary>
    /// 특정 아이템을 보유하고 있는지 확인합니다.
    /// </summary>
    /// <param name="item">확인할 아이템</param>
    /// <returns>보유하고 있으면 true</returns>
    public bool HasItem(Item item)
    {
        if (item == null) return false;
        return items.Contains(item);
    }

    #endregion

    #region Debug

    /// <summary>
    /// 디버그: 인벤토리 아이템 목록을 출력합니다.
    /// </summary>
    public void PrintItems()
    {
        Debug.Log("=== Inventory Items ===");
        string logs = "";
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            uint quantity = 0;
            if (slots != null && i < slots.Length)
                quantity = slots[i].ItemQuantity;

            if (item != null)
            {
                logs += $"Slot {i}: {item.itemName} (x{quantity})\n";
            }
            else
            {
                logs += $"Slot {i}: (empty)\n";
            }
        }
        Debug.Log(logs + "=======================");
    }

    #endregion
}
