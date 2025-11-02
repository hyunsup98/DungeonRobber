using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 일반 인벤토리 슬롯
/// </summary>
public class InvenSlot : Slot
{
    protected override void Awake()
    {
        slotType = SlotType.Inventory;
        base.Awake(); // 부모의 Awake 호출 (FindOwner 포함)
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭: 컨텍스트 메뉴 표시
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_item == null)
                return;

            var menu = InventoryContextMenu.GetOrFind();
            if (menu != null)
            {
                menu.Show(this, eventData.position);
            }
        }
    }
}
