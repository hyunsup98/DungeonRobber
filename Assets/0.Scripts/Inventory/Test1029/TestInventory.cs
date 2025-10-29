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
            Debug.Log("������ ���� �� �ֽ��ϴ�.");
        }
    }

    /// <summary>
    /// idx��° ������ �������� ��ȯ�մϴ�.
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
        Debug.Log($"�κ��丮 {idx+1}��° �������� ���ŵǾ����ϴ�.");
    }
}
