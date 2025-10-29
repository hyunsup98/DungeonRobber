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
    [Tooltip("�κ��丮 ��ü UI ��Ʈ(Ȱ��/��Ȱ�� ���)")]
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
            // ���Կ� owner / index �Ҵ�
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
            Debug.Log("������ ���� �� �ֽ��ϴ�.");
        }
    }

    /// <summary>
    /// idx��° ������ �������� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public Item GetItem(int idx)
    {
        return slots[idx].item;
    }

    // ���� �ε��� ��� ���� (����)
    internal void RemoveItem(int idx)
    {
        if (idx >= 0 && idx < items.Count)
        {
            items.RemoveAt(idx);
            FreshSlot();
            Debug.Log($"�κ��丮 {idx + 1}��° �������� ���ŵǾ����ϴ�.");
        }
    }

    // Item ��� ���� (������ ��� ȣ���)
    internal void RemoveItem(Item item)
    {
        if (item == null) return;

        if (items.Remove(item))
        {
            FreshSlot();
            Debug.Log($"�κ��丮 '{item.itemName}' �������� ���ŵǾ����ϴ�.");
        }
        else
        {
            Debug.LogWarning("RemoveItem: �ش� �������� �κ��丮�� �����ϴ�.");
        }
    }
}
