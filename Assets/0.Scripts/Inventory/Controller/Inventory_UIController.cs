using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Inventory_UIController
{
    [Header("UI")]
    [Tooltip("�κ��丮 ��ü UI ��Ʈ(Ȱ��/��Ȱ�� ���)")]
    [SerializeField] private GameObject inventoryRoot;

    /// <summary>
    /// �κ��丮 ���� ���θ� ��ȯ�մϴ�.
    /// </summary>
    public bool IsOpen => inventoryRoot != null && inventoryRoot.activeSelf;

    /// <summary>
    /// �κ��丮 ����/������ ����մϴ�.
    /// </summary>
    public void ToggleInventory()
    {
        if (IsOpen)
        {
            HideInventory();
        }
        else
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
        //FreshSlot();
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
        //ItemTooltip itemTooltip = ItemTooltip.GetOrFind();
        //if (itemTooltip != null)
        //    itemTooltip.Hide();

        if (inventoryRoot != null) inventoryRoot.SetActive(false);
    }


    //public void RefreshSlot()
    //{
    //    if (slotParent == null)
    //        return;

    //    if (slots == null || slots.Length == 0)
    //        slots = slotParent.GetComponentsInChildren<Slot>();

    //    int i = 0;
    //    for (; i < slots.Length; i++)
    //    {
    //        // ���Կ� owner / index �Ҵ�
    //        slots[i].ownerInventory = this;
    //        slots[i].Item = items[i];
    //        // ���� �ؽ�Ʈ ����
    //        if (slots[i].ItemQuantity > 0)
    //            slots[i].GetComponentInChildren<Text>().text = slots[i].ItemQuantity.ToString();
    //        else
    //            slots[i].GetComponentInChildren<Text>().text = "";
    //    }
    //}
}
