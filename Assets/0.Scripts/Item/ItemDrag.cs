using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 아이템 드래그 앤 드롭 시스템
/// 인벤토리, 퀵슬롯, 장비 슬롯 간 아이템 이동 지원
/// </summary>
public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image dragImage;
    private Slot slot;

    void Start()
    {
        // 현재 슬롯의 컨트롤러를 가져옵니다 (자식에서도 찾기)
        slot = GetComponent<Slot>() ?? GetComponentInChildren<Slot>();
        
        // 드래그 이미지를 태그를 활용해서 찾습니다
        GameObject dragImageObj = GameObject.FindGameObjectWithTag("DragImage");
        if (dragImageObj != null)
        {
            dragImage = dragImageObj.GetComponent<Image>();
        }
        else
        {
            Debug.LogWarning("DragImage를 찾을 수 없습니다! 'DragImage' 태그가 설정되어 있는지 확인하세요.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 슬롯에 아이템이 없으면 드래그 불가
        if (slot == null || slot.Item == null || dragImage == null)
            return;

        // 드래그 이미지 활성화
        dragImage.gameObject.SetActive(true);

        // 드래그 이미지 크기를 설정합니다
        float size = GetComponent<RectTransform>().sizeDelta.x;
        dragImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        dragImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

        // 드래그 이미지에 아이템 스프라이트 설정
        dragImage.sprite = slot.Item.itemImage;
        dragImage.color = new Color(1, 1, 1, 0.8f); // 약간 투명하게
        
        // 슬롯 이미지 반투명 처리
        Image slotImage = GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.color = new Color(slotImage.color.r, slotImage.color.g, slotImage.color.b, 0.5f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 이미지가 없거나 슬롯에 아이템이 없으면 중단
        if (slot == null || slot.Item == null || dragImage == null)
            return;

        // 드래그 이미지 위치를 마우스 위치로 이동
        dragImage.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slot == null || slot.Item == null || dragImage == null)
        {
            EndDragVisuals();
            return;
        }

        // UI GraphicRaycaster로 Raycast
        if (EventSystem.current != null)
        {
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            // dragImage의 부모 캔버스로부터 GraphicRaycaster를 찾아서 Raycast 수행
            GraphicRaycaster gr = null;
            if (dragImage != null)
                gr = dragImage.GetComponentInParent<GraphicRaycaster>();
            if (gr == null)
                gr = FindObjectOfType<GraphicRaycaster>();

            if (gr != null)
            {
                gr.Raycast(ped, results);
                
                foreach (var res in results)
                {
                    Slot targetSlot = res.gameObject.GetComponent<Slot>() ?? res.gameObject.GetComponentInParent<Slot>();

                    if (targetSlot != null && targetSlot != slot)
                    {
                        Debug.Log($"슬롯에 드롭: {slot.name} -> {targetSlot.name}");
                        SwapSlots(slot, targetSlot);
                        break;
                    }
                }
            }
        }

        // 드래그 종료 처리
        EndDragVisuals();
    }

    /// <summary>
    /// 두 슬롯의 아이템을 교환합니다
    /// 인벤토리 <-> 퀵슬롯 간 교환 지원
    /// </summary>
    private void SwapSlots(Slot fromSlot, Slot toSlot)
    {
        if (fromSlot == null || toSlot == null)
            return;

        Item fromItem = fromSlot.Item;
        Item toItem = toSlot.Item;

        // 같은 소유자의 슬롯들 간 교환 (인벤토리 <-> 인벤토리 또는 퀵슬롯 <-> 퀵슬롯)
        if (fromSlot.ownerInventory != null && toSlot.ownerInventory != null && 
            fromSlot.ownerInventory == toSlot.ownerInventory)
        {
            // 인벤토리 내부 교환
            fromSlot.ownerInventory.SwapItems(fromSlot.Index, toSlot.Index);
            return;
        }
        
        if (fromSlot.ownerQuickSlots != null && toSlot.ownerQuickSlots != null &&
            fromSlot.ownerQuickSlots == toSlot.ownerQuickSlots)
        {
            // 퀵슬롯 내부 교환
            fromSlot.ownerQuickSlots.SwapItems(fromSlot.Index, toSlot.Index);
            return;
        }

        // 서로 다른 시스템 간 교환 (인벤토리 <-> 퀵슬롯)
        // 퀵슬롯은 인벤토리의 링크/참조 역할
        if ((fromSlot.ownerInventory != null && toSlot.ownerQuickSlots != null) ||
            (fromSlot.ownerQuickSlots != null && toSlot.ownerInventory != null))
        {
            // 인벤토리 -> 퀵슬롯
            if (fromSlot.ownerInventory != null && toSlot.ownerQuickSlots != null)
            {
                // 기존 퀵슬롯 아이템 처리
                if (toItem != null)
                {
                    // 퀵슬롯에 있던 아이템은 그냥 클리어
                    fromSlot.ownerQuickSlots.items[toSlot.Index] = null;
                }
                // 퀵슬롯에 인벤토리 아이템 링크 설정
                fromSlot.ownerQuickSlots.items[toSlot.Index] = fromItem;
                fromSlot.ownerQuickSlots.RefreshSlots();
                Debug.Log($"인벤토리 -> 퀵슬롯: '{fromItem?.itemName}' 등록");
            }
            // 퀵슬롯 -> 인벤토리
            else if (fromSlot.ownerQuickSlots != null && toSlot.ownerInventory != null)
            {
                // 퀵슬롯 등록 해제
                fromSlot.ownerQuickSlots.items[fromSlot.Index] = null;
                fromSlot.ownerQuickSlots.RefreshSlots();
                
                // 인벤토리에 아이템이 있으면 교환
                if (toItem != null)
                {
                    // 인벤토리 아이템을 퀵슬롯에 등록
                    fromSlot.ownerQuickSlots.items[fromSlot.Index] = toItem;
                    fromSlot.ownerQuickSlots.RefreshSlots();
                }
                Debug.Log($"퀵슬롯 -> 인벤토리: '{fromItem?.itemName}' 등록 해제");
            }
            
            Debug.Log($"인벤토리 <-> 퀵슬롯 교환: {fromItem?.itemName ?? "빈 슬롯"} <-> {toItem?.itemName ?? "빈 슬롯"}");
            return;
        }

        Debug.LogWarning("슬롯 교환 실패: 호환되지 않는 슬롯 타입");
    }

    /// <summary>
    /// 드래그 종료 시 시각적 복원
    /// </summary>
    private void EndDragVisuals()
    {
        if (dragImage != null)
        {
            // 드래그 이미지 비활성화
            dragImage.gameObject.SetActive(false);
        }

        // 슬롯 이미지 복원
        if (slot != null)
        {
            Image slotImage = GetComponent<Image>();
            if (slotImage != null && slot.Item != null)
            {
                slotImage.color = new Color(slotImage.color.r, slotImage.color.g, slotImage.color.b, 1f);
            }
        }
    }
}
