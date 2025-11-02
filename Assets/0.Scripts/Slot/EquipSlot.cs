using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 장비 슬롯 - Weapon/Equipment만 장착 가능
/// </summary>
public class EquipSlot : Slot
{
    protected override void Awake()
    {
        slotType = SlotType.Equipment; // 장비 슬롯 타입으로 설정
        base.Awake(); // 부모의 Awake 호출 (FindOwner 포함)
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭: 장비 해제 컨텍스트 메뉴 표시
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_item == null) return;

            var menu = EquipContextMenu.GetOrFind();
            if (menu != null)
            {
                menu.Show(this, eventData.position);
            }
        }
    }
    
    /// <summary>
    /// 아이템이 장비 가능한 타입인지 확인
    /// </summary>
    public bool CanEquip(Item item)
    {
        return item != null && (item.itemType == Item.ItemType.Weapon || item.itemType == Item.ItemType.Equipment);
    }
}
