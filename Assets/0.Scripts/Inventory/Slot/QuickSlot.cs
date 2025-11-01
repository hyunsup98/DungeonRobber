using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class QuickSlot : Slot, IPointerClickHandler
{
    //public QuickSlot_Controller ownerQuickSlots;

    void Start()
    {
        slotType = SlotType.QuickSlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // ��Ŭ��: ���ؽ�Ʈ �޴� ǥ��
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"_item: {_item}");
            if (_item == null) return;

            var menu = QuickSlotsContextMenu.GetOrFind();
            if (menu != null)
            {
                menu.Show(this, eventData.position);
            }
        }
    }
}
