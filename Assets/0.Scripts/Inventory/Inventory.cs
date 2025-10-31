using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    // ���� ���� ������ ����Ʈ�� �ҷ��ͼ� UI�� ǥ��
    // EŰ�� ������ �κ��丮 â�� ������ ������ ��� < Item/Player.cs���� ó�� >

    [SerializeField] int invenCapacity = 10;

    private GameObject player;
    private Item[] invenArray;

    private void Awake()
    {
        // �κ��丮 9ĭ
        items = new List<Item>() { null, null, null, null, null, null, null, null, null };
        FreshSlot();
        HideInventory();
    }

    /// <summary>
    /// �κ��丮 ���� ���θ� ��ȯ�մϴ�.
    /// </summary>
    public bool IsOpen => inventoryRoot != null && inventoryRoot.activeSelf;

    /// <summary>
    /// �κ��丮 ����/������ ����մϴ�.
    /// </summary>
    public void ToggleInventory()
    {
        TextMeshProUGUI invenText = GameObject.FindGameObjectWithTag("InvenText")?.GetComponent<TextMeshProUGUI>();
        invenText.text = $"Inventory: {String.Join(" / ", Array.ConvertAll(invenArray, item => item != null ? item.Name : " "))}";
    }

    /// <summary>
    /// ������ ���� Ȯ��
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns>�������� bool</returns>
    public bool HasItem(string itemName)
    {
        foreach (var invItem in invenArray)
        {
            if (invItem != null && invItem.Name == itemName)
            {
                return true;
            }
        }
        return false;
    }

    public void AddItem(Item newItem)
    {
        if (HasItem(newItem.Name))
        {
            ShowInventory();
        }
    }

    /// <summary>
    /// �κ��丮�� ǥ���մϴ�.
    /// </summary>
    public void ShowInventory()
    {
        if (inventoryRoot != null) inventoryRoot.SetActive(true);
        FreshSlot();
    }

    /// <summary>
    /// �κ��丮�� ����ϴ�.
    /// </summary>
    public void HideInventory()
    {
        // ���ؽ�Ʈ �޴� ����
        InventoryContextMenu inventoryContextMenu = InventoryContextMenu.GetOrFind();
        if (inventoryContextMenu != null)
            inventoryContextMenu.Hide();

        // ���� ���� (������ �κ��丮 ���Կ� ���� ���� ����� �� ���� ���� ����)
        ItemTooltip itemTooltip = ItemTooltip.GetOrFind();
        if (itemTooltip != null)
            itemTooltip.Hide();

        if (inventoryRoot != null) inventoryRoot.SetActive(false);
    }

    /// <summary>
    /// �κ��丮 ���� UI�� �����մϴ�.
    /// </summary>
    public void FreshSlot()
    {
        if (slotParent == null)
            return;

        if (slots == null || slots.Length == 0)
            slots = slotParent.GetComponentsInChildren<Slot>();

        int i = 0;
        for (; i < slots.Length; i++)
        {
            // ���Կ� owner / index �Ҵ�
            slots[i].ownerInventory = this;
            slots[i].Item = items[i];
            // ���� �ؽ�Ʈ ����
            if(slots[i].ItemQuantity > 0)
                slots[i].GetComponentInChildren<Text>().text = slots[i].ItemQuantity.ToString();
            else
                slots[i].GetComponentInChildren<Text>().text = "";
        }
        //for (; i < slots.Length; i++)
        //{
        //    slots[i].ownerInventory = this;
        //    slots[i].Item = null;
        //}

        



        PrintItems();
    }

    public enum SlotType
    {
        Inventory,
        QuickSlots
    }

    /// <summary>
    /// �κ��丮 �� ������ �� ������ ��ȯ �Ǵ� ���/������ ó���մϴ�.
    /// </summary>
    /// <param name="from">�������� �������� ��</param>
    /// <param name="fromIdx"></param>
    /// <param name="to">�������� ���� ��</param>
    /// <param name="toIdx"></param>
    public void SwapItem(SlotType from, int fromIdx, SlotType to, int toIdx)
    {
        Debug.Log($"SwapItem: �Ķ���� idx1: {fromIdx}, idx2: {toIdx}");
        
        // ��ȿ�� �˻�
        if (slotParent == null) return;
        if (fromIdx < 0 || fromIdx >= items.Count || toIdx < 0 || toIdx >= items.Count)
        {
            Debug.LogWarning("SwapItem: �ε����� ��ȿ���� �ʽ��ϴ�.");
            return;
        }
        
        if (from == SlotType.Inventory && to == SlotType.Inventory)
        {
            // �κ��丮 �� ��ȯ
            var tempSlot = slots[fromIdx];
            slots[fromIdx] = slots[toIdx];
            slots[toIdx] = tempSlot;

            //var tempItem = items[fromIdx];
            //items[fromIdx] = items[toIdx];
            //items[toIdx] = tempItem;
        }
        else if (from == SlotType.Inventory && to == SlotType.QuickSlots)
        {
            // �κ��丮 -> �������� ���
            QuickSlot_Controller quickSlots = FindObjectOfType<QuickSlot_Controller>();
            if (quickSlots == null)
            {
                Debug.LogWarning("SwapItem: QuickSlots ������Ʈ�� ã�� �� �����ϴ�.");
                return;
            }
            quickSlots.items[toIdx] = items[fromIdx];
            quickSlots.FreshSlot();
        }
        else if (from == SlotType.QuickSlots && to == SlotType.Inventory)
        {
            // ������ -> �κ��丮�� ��� ����
            QuickSlot_Controller quickSlots = FindObjectOfType<QuickSlot_Controller>();
            if (quickSlots == null)
            {
                Debug.LogWarning("SwapItem: QuickSlots ������Ʈ�� ã�� �� �����ϴ�.");
                return;
            }
            quickSlots.RemoveItem(quickSlots.items[fromIdx]);
        }
        else if (from == SlotType.Inventory && to == SlotType.Inventory)
        {
            // ������ �� ��ȯ
            QuickSlot_Controller quickSlots = FindObjectOfType<QuickSlot_Controller>();
            if (quickSlots == null)
            {
                Debug.LogWarning("SwapItem: QuickSlots ������Ʈ�� ã�� �� �����ϴ�.");
                return;
            }
            var temp = quickSlots.items[fromIdx];
            quickSlots.items[fromIdx] = quickSlots.items[toIdx];
            quickSlots.items[toIdx] = temp;
            quickSlots.FreshSlot();
        }
        else
        {
            Debug.LogWarning("SwapItem: �߸��� from/to �Ķ�����Դϴ�.");
            return;
        }

        FreshSlot();
    }

    //public bool AddItem(int idx, Item item)
    //{
    //    if (slots == null || items == null)
    //        FreshSlot();

    //    if (idx < 0 || idx >= items.Count)
    //    {
    //        Debug.Log("AddItem: �ε����� ��ȿ���� �ʽ��ϴ�.");
    //        return false;
    //    }

    //    items[idx] = item;
    //    FreshSlot();
    //    Debug.Log($"�κ��丮�� '{item.itemName}' �������� �߰��Ǿ����ϴ�.");
    //    return true;
    //}

    public bool AddItem(Item item)
    {
        speedRune = gameObject.AddComponent<ConsumableItem>();
        speedRune.Name = "Speed Rune";
        speedRune.Type = ItemType.Consumable;
        speedRune.Grade = ItemGrade.Rare;
        speedRune.Description = "����ϸ� ���� �ð� ���� �̵� �ӵ��� �������� ��. ���� Ž���̳� ���� ���� ���� �����ϴ�.";
        speedRune.ConsumeType = ConsumableType.Effect;
        speedRune.Power = 2f;
        speedRune.Duration = 5;

        // ������ �ش� �������� �ִ��� �˻�
        if (items.Find(x => x == item) != null)
        {
            // ������ ���� ����
            int itemIdx = items.FindIndex(x => x == item);
            Debug.Log($"�κ��丮�� �̹� '{item.itemName}' �������� �ֽ��ϴ�. ������ ������ŵ�ϴ�.");
            // ������ ���� ����
            slots[itemIdx].ItemQuantity++;
            FreshSlot();
            return true;
        }

        // ������ ������ �� ĭ ã�Ƽ� �߰�
        int idx = items.FindIndex(x => x == null);
        if (idx != -1)
        {
            items[idx] = item;
            Debug.Log($"�κ��丮�� '{item.itemName}' �������� �߰��Ǿ����ϴ�.");
            slots[idx].ItemQuantity++;
            FreshSlot();
            return true;
        }
        else
        {
            Debug.Log("������ ���� �� �ֽ��ϴ�.");
            return false;
        }
    }

        Item[] randomItems = new Item[] { speedRune, necklace };

    // ���� �ε��� ��� ���� (����)
    internal void RemoveItem(int idx)
    {
        if (idx >= 0 && idx < items.Count)
        {
            items[idx] = null;
            FreshSlot();
            Debug.Log($"�κ��丮 {idx + 1}��° �������� ���ŵǾ����ϴ�.");
        }
    }

    /// <summary>
    /// Item ��� ���� (������ ��� ȣ���)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    internal void RemoveItem(Item item)
    {
        if (item == null) return;

        int idx = items.FindIndex(x => x == item);
        if (idx != -1) {
            items[idx] = null;
            FreshSlot();
            Debug.Log($"�κ��丮 '{item.itemName}' �������� ���ŵǾ����ϴ�.");
            return;
        }
        else
        {
            Debug.LogWarning("RemoveItem: �ش� �������� �κ��丮�� �����ϴ�.");
        }
    }

    // ����׿�: �κ��丮 ������ ���
    public void PrintItems()
    {
        Debug.Log("=== Inventory Items ===");
        String logs = "";
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item != null)
            {
                logs += $"Slot {i}: {item.itemName}({slots[i].ItemQuantity})\n";
            }
            else
            {
                logs += $"Slot {i}: (empty)\n";
            }
        }
        Debug.Log(logs+"\n=======================");
    }
}
