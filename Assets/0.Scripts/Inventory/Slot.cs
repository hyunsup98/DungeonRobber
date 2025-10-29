using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image image;

    // 슬롯 소유자(인벤토리 / 퀵슬롯)와 인덱스 정보
    public Inventory ownerInventory;
    public QuickSlots ownerQuickSlots;
    public int slotIndex = -1;

    private Item _item;
    public Item item
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

    // 우클릭 처리: 인벤토리 슬롯이면 컨텍스트 메뉴 표시
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Slot OnPointerClick");
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"_item: {_item}");
            if (_item == null) return;

            var menu = InventoryContextMenu.GetOrFind();
            if (menu != null)
            {
                menu.Show(this, eventData.position);
            }
        }
    }

    // 디버그용 포인터 다운/업 이벤트
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"OnPointerDown on {gameObject.name}  pressObject: {eventData.pointerPress?.name}");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"OnPointerUp on {gameObject.name}  currentRaycast: {eventData.pointerCurrentRaycast.gameObject?.name}");
    }
}