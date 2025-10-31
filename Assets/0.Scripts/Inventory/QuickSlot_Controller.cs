using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot_Controller : MonoBehaviour
{
    [Header("�κ��丮")]
    public Inventory inventory;

    public List<Item> items;

    [SerializeField]
    private Transform slotParent;
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
        items = new List<Item>() { null, null, null, null, null, null };
        FreshSlot();
    }

    public void FreshSlot()
    {
        if (slotParent == null)
            return;

        if (slots == null || slots.Length == 0)
            slots = slotParent.GetComponentsInChildren<QuickSlot>();

        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            // ������ ������/�ε��� ����
            //slots[i].ownerQuickSlots = this;
            slots[i].Item = items[i];
        }
        for (; i < slots.Length; i++)
        {
            //slots[i].ownerQuickSlots = this;
            slots[i].Item = null;
        }
    }

    // �������� 6ĭ���� �������ִ�
    // ���� ó�� null�� ĭ�� ������ �߰�
    public void AddItem(Item item)
    {
        // �� ĭ ã��
        int idx = items.FindIndex(x => x == null);
        if (idx != -1)
        {
            items[idx] = item;
            FreshSlot();
        }
        else
        {
            Debug.Log("�������� ���� �� �ֽ��ϴ�.");
        }
    }

    public Item GetItem(int idx)
    {
        return slots[idx].Item;
    }
    
    // ������ ������ ��� �� ����
    internal void RemoveItem(int idx)
    {
        if (idx < 0 || idx >= items.Count) return;
        inventory?.RemoveItem(items[idx]);
        Debug.Log($"{items[idx]}�� ���ŵǾ����ϴ�.");
        items[idx] = null;
        FreshSlot();
    }

    // ������ ��� ����
    internal void RemoveItem(Item item)
    {
        int idx = items.FindIndex(x => x == item);
        Debug.Log($"�����Կ��� {item.name}�� ���ŵǾ����ϴ�.");
        items[idx] = null;
        FreshSlot();
    }
}
