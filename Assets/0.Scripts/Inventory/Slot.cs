using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 개별 슬롯 UI를 담당합니다.
/// private 필드: _item
/// public 프로퍼티: Item (UI 갱신)
/// </summary>
public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image image;

    // 슬롯 소유자(인벤토리 / 퀵슬롯)와 인덱스 정보
    public Inventory ownerInventory;
    public QuickSlots ownerQuickSlots;
    public int slotIndex = -1;

    // 내부 필드
    private Item _item;

    /// <summary>
    /// 슬롯에 세팅된 아이템. set에서 UI 이미지 갱신 처리.
    /// </summary>
    public Item Item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item != null)
            {
                image.sprite = _item.itemImage;
                image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                image.color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_item != null && ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.Show(_item);
            ItemTooltip.Instance.SetPosition(eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemTooltip.Instance != null)
            ItemTooltip.Instance.Hide();
    }

    // 우클릭 처리: 인벤토리에서 컨텍스트 메뉴 표시
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"_item: {_item}");
            if (_item == null) return;


            // 퀵슬롯일 경우
            if (ownerQuickSlots != null)
            {
                var menu = QuickSlotsContextMenu.GetOrFind();
                if (menu != null)
                {
                    menu.Show(this, eventData.position);
                }
            }
            else
            {
                var menu = InventoryContextMenu.GetOrFind();
                if (menu != null)
                {
                    menu.Show(this, eventData.position);
                }
            }


        }
    }

    // 디버그용 포인터 다운/업 이벤트
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log($"OnPointerDown on {gameObject.name}  pressObject: {eventData.pointerPress?.name}");
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    Debug.Log($"OnPointerUp on {gameObject.name}  currentRaycast: {eventData.pointerCurrentRaycast.gameObject?.name}");
    //}
}