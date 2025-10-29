using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    public List<Item> items;

    [SerializeField]
    private Transform slotParent;
    [SerializeField] private Transform inventoryParent;
    private Slot[] slots;

    [Header("UI")]
    [Tooltip("인벤토리 전체 UI 루트(활성/비활성 토글)")]
    [SerializeField] private GameObject inventoryRoot;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (slotParent != null)
            slots = slotParent.GetComponentsInChildren<Slot>();
    }
#endif

    void Awake()
    {
        FreshSlot();
        HideInventory();
    }

    public bool IsOpen => inventoryRoot != null ? inventoryRoot.activeSelf : gameObject.activeSelf;

    public void ToggleInventory()
    {
        if (inventoryRoot != null)
        {
            bool open = !inventoryRoot.activeSelf;
            inventoryRoot.SetActive(open);
            if (open) FreshSlot();
        }
        else
        {
            bool open = !gameObject.activeSelf;
            gameObject.SetActive(open);
            if (open) FreshSlot();
        }
    }

    public void ShowInventory()
    {
        if (inventoryRoot != null) inventoryRoot.SetActive(true);
        else gameObject.SetActive(true);
        FreshSlot();
    }

    public void HideInventory()
    {
        if (inventoryRoot != null) inventoryRoot.SetActive(false);
        else gameObject.SetActive(false);
    }

    public void FreshSlot()
    {
        if (slotParent == null)
            return;

        if (slots == null || slots.Length == 0)
            slots = slotParent.GetComponentsInChildren<Slot>();

        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            // 슬롯에 owner / index 할당
            slots[i].ownerInventory = this;
            slots[i].slotIndex = i;
            slots[i].item = items[i];
        }
        for (; i < slots.Length; i++)
        {
            slots[i].ownerInventory = this;
            slots[i].slotIndex = i;
            slots[i].item = null;
        }
    }

    public void AddItem(Item _item)
    {
        if (slots == null || items == null)
            FreshSlot();

        if (items.Count < slots.Length)
        {
            items.Add(_item);
            FreshSlot();
        }
        else
        {
            Debug.Log("슬롯이 가득 차 있습니다.");
        }
    }

    /// <summary>
    /// idx번째 슬롯의 아이템을 반환합니다.
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public Item GetItem(int idx)
    {
        return slots[idx].item;
    }

    // 기존 인덱스 기반 제거 (유지)
    internal void RemoveItem(int idx)
    {
        if (idx >= 0 && idx < items.Count)
        {
            items.RemoveAt(idx);
            FreshSlot();
            Debug.Log($"인벤토리 {idx + 1}번째 아이템이 제거되었습니다.");
        }
    }

    // Item 기반 제거 (퀵슬롯 등에서 호출됨)
    internal void RemoveItem(Item item)
    {
        if (item == null) return;

        if (items.Remove(item))
        {
            FreshSlot();
            Debug.Log($"인벤토리 '{item.itemName}' 아이템이 제거되었습니다.");
        }
        else
        {
            Debug.LogWarning("RemoveItem: 해당 아이템이 인벤토리에 없습니다.");
        }
    }
}
