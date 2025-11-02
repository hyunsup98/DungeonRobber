using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 퀵 슬롯
/// </summary>
public class QuickSlot : Slot
{
    protected override void Awake()
    {
        slotType = SlotType.QuickSlot;
        base.Awake(); // 부모의 Awake 호출 (FindOwner 포함)
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭: 컨텍스트 메뉴 표시
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_item == null) return;

            var menu = QuickSlotsContextMenu.GetOrFind();
            if (menu != null)
            {
                menu.Show(this, eventData.position);
            }
        }
    }
}
