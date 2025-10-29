using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestInventory : MonoBehaviour
{
    public List<ItemModel> items;

    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private Slot2[] slots;

#if UNITY_EDITOR
    private void OnValidate()
    {
        slots = slotParent.GetComponentsInChildren<Slot2>();
    }
#endif

    void Awake()
    {
        FreshSlot();
    }

    public void FreshSlot()
    {
        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            slots[i].item = items[i];
        }
        for (; i < slots.Length; i++)
        {
            slots[i].item = null;
        }
    }

    public void AddItem(ItemModel _item)
    {
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
    public ItemModel GetItem(int idx)
    {
        return slots[idx].item;
    }

    internal void RemoveItem(int idx)
    {
        items.RemoveAt(idx);
        FreshSlot();
        Debug.Log($"인벤토리 {idx+1}번째 아이템이 제거되었습니다.");
    }
}
