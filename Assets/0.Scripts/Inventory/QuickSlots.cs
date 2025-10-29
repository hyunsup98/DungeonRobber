using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlots : MonoBehaviour
{
    [Header("�κ��丮")]
    public Inventory inventory;

    public List<Item> items;

    [SerializeField]
    private Transform slotParent;
    private Slot[] slots;

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
            // ������ ������/�ε��� ����
            slots[i].ownerQuickSlots = this;
            slots[i].slotIndex = i;
            slots[i].item = items[i];
        }
        for (; i < slots.Length; i++)
        {
            slots[i].ownerQuickSlots = this;
            slots[i].slotIndex = i;
            slots[i].item = null;
        }
    }

    public void AddItem(Item _item)
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

    public Item GetItem(int idx)
    {
        return slots[idx].item;
    }

    internal void RemoveItem(int idx)
    {
        if (idx < 0 || idx >= items.Count) return;
        inventory?.RemoveItem(items[idx]);
        items.RemoveAt(idx);
        FreshSlot();
        Debug.Log($"������ {idx + 1}��° �������� ���ŵǾ����ϴ�.");
    }
}
