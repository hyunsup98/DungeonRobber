using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Inventory_UIController
{
    [Header("UI")]
    [Tooltip("인벤토리 전체 UI 루트(활성/비활성 토글)")]
    [SerializeField] private GameObject inventoryRoot;

    /// <summary>
    /// 인벤토리 열림 여부를 반환합니다.
    /// </summary>
    public bool IsOpen => inventoryRoot != null && inventoryRoot.activeSelf;

    /// <summary>
    /// 인벤토리 열림/닫힘을 토글합니다.
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
    /// 인벤토리를 표시합니다.
    /// </summary>
    public void ShowInventory()
    {
        if (inventoryRoot != null) inventoryRoot.SetActive(true);
        //FreshSlot();
    }

    /// <summary>
    /// 인벤토리를 숨깁니다.
    /// </summary>
    public void HideInventory()
    {
        // 컨텍스트 메뉴 숨김
        InventoryContextMenu inventoryContextMenu = InventoryContextMenu.GetOrFind();
        if (inventoryContextMenu != null)
            inventoryContextMenu.Hide();

        // 툴팁 숨김 (툴팁이 인벤토리 슬롯에 있을 때만 숨기는 게 나을 수도 있음)
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
    //        // 슬롯에 owner / index 할당
    //        slots[i].ownerInventory = this;
    //        slots[i].Item = items[i];
    //        // 수량 텍스트 갱신
    //        if (slots[i].ItemQuantity > 0)
    //            slots[i].GetComponentInChildren<Text>().text = slots[i].ItemQuantity.ToString();
    //        else
    //            slots[i].GetComponentInChildren<Text>().text = "";
    //    }
    //}
}
