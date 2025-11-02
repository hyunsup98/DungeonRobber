using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 상자 슬롯
/// </summary>
public class ChestSlot : Slot
{
    protected override void Awake()
    {
        slotType = SlotType.Chest; // 상자 슬롯 타입으로 설정
        base.Awake(); // 부모의 Awake 호출 (FindOwner 포함)
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭: 아이템 얻기 컨텍스트 메뉴 표시
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_item == null) return;

            var menu = ChestContextMenu.GetOrFind();
            if (menu != null)
            {
                menu.Show(this, eventData.position);
            }
        }
    }
}

