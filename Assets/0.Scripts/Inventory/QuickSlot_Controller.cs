using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 퀵 슬롯 컨트롤러
/// </summary>
public class QuickSlot_Controller : MonoBehaviour
{
    [Header("QuickSlots Settings")]
    [SerializeField] private int quickSlotSize = 6;

    [Header("Inventory")]
    public Inventory inventory;

    [Header("UI References")]
    [SerializeField] private Transform slotParent;

    public List<Item> items;
    private QuickSlot[] slots;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (slotParent != null)
            slots = slotParent.GetComponentsInChildren<QuickSlot>();
    }
#endif

    void Awake()
    {
        // 퀵 슬롯 크기만큼 null로 초기화
        items = new List<Item>();
        for (int i = 0; i < quickSlotSize; i++)
        {
            items.Add(null);
        }
        
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        if (slotParent == null)
        {
            Debug.LogWarning("QuickSlot_Controller: slotParent가 설정되지 않았습니다.");
            return;
        }

        slots = slotParent.GetComponentsInChildren<QuickSlot>();
        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning("QuickSlot_Controller: 슬롯을 찾을 수 없습니다.");
            return;
        }

        // 슬롯에 초기화
        for (int i = 0; i < slots.Length && i < items.Count; i++)
        {
            slots[i].ownerInventory = inventory;
            slots[i].ownerQuickSlots = this;
            slots[i].Item = items[i];
        }
    }

    /// <summary>
    /// 퀵 슬롯을 새로고침합니다.
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
    /// 아이템을 퀵 슬롯에 추가합니다.
    /// </summary>
    public void AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("QuickSlot_Controller: null 아이템은 추가할 수 없습니다.");
            return;
        }

        // 빈 슬롯 찾기
        int idx = items.FindIndex(x => x == null);
        if (idx != -1)
        {
            items[idx] = item;
            RefreshSlots();
            Debug.Log($"퀵 슬롯에 '{item.itemName}' 아이템이 추가되었습니다.");
        }
        else
        {
            Debug.LogWarning("퀵 슬롯이 가득 찼습니다.");
        }
    }

    /// <summary>
    /// 특정 인덱스의 아이템을 반환합니다.
    /// </summary>
    /// <param name="idx">슬롯 인덱스</param>
    /// <returns>아이템 또는 null</returns>
    public Item GetItem(int idx)
    {
        if (idx < 0 || idx >= items.Count || slots == null || idx >= slots.Length)
            return null;
        
        return slots[idx].Item;
    }

    /// <summary>
    /// 아이템을 퀵 슬롯에서 제거합니다 (인덱스).
    /// </summary>
    internal void RemoveItem(int idx)
    {
        if (idx < 0 || idx >= items.Count)
        {
            Debug.LogWarning($"QuickSlot_Controller: 잘못된 인덱스 {idx}");
            return;
        }

        Item removedItem = items[idx];
        items[idx] = null;
        RefreshSlots();
        Debug.Log($"퀵 슬롯에서 {removedItem?.itemName ?? "아이템"}이 제거되었습니다.");
    }

    /// <summary>
    /// 아이템을 퀵 슬롯에서 제거합니다 (아이템 오브젝트).
    /// </summary>
    internal void RemoveItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("QuickSlot_Controller: null 아이템은 제거할 수 없습니다.");
            return;
        }

        int idx = items.FindIndex(x => x == item);
        if (idx != -1)
        {
            items[idx] = null;
            RefreshSlots();
            Debug.Log($"퀵 슬롯에서 '{item.itemName}' 아이템이 제거되었습니다.");
        }
        else
        {
            Debug.LogWarning("QuickSlot_Controller: 해당 아이템이 퀵 슬롯에 없습니다.");
        }
    }

    /// <summary>
    /// 두 슬롯의 아이템을 교환합니다.
    /// </summary>
    public void SwapItems(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= items.Count || toIdx < 0 || toIdx >= items.Count)
        {
            Debug.LogWarning("QuickSlot_Controller: 잘못된 인덱스입니다.");
            return;
        }

        var tempItem = items[fromIdx];
        items[fromIdx] = items[toIdx];
        items[toIdx] = tempItem;

        RefreshSlots();
        Debug.Log($"퀵 슬롯 {fromIdx + 1}과 {toIdx + 1}의 아이템을 교환했습니다.");
    }
}
