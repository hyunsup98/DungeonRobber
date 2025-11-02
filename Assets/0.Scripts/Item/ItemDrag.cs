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

    private static Image cachedDragImage;
    private static bool dragImageFound = false;

    void Start()
    {
        // 현재 슬롯의 컨트롤러를 가져옵니다 (자식에서도 찾기)
        slot = GetComponent<Slot>() ?? GetComponentInChildren<Slot>();
        
        // DragImage 캐싱 (한 번만 찾기)
        if (!dragImageFound)
        {
            FindDragImage();
        }
        
        dragImage = cachedDragImage;
    }

    /// <summary>
    /// DragImage 찾기 (비활성화된 오브젝트 포함)
    /// </summary>
    private static void FindDragImage()
    {
        // 먼저 활성화된 오브젝트에서 찾기
        GameObject dragImageObj = GameObject.FindGameObjectWithTag("DragImage");
        
        // 찾지 못하면 Canvas 자식에서 재귀적으로 검색
        if (dragImageObj == null)
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>(true); // 비활성화 포함
            foreach (Canvas canvas in allCanvases)
            {
                dragImageObj = FindDragImageInChildren(canvas.transform, "DragImage");
                if (dragImageObj != null)
                    break;
            }
        }
        
        if (dragImageObj != null)
        {
            cachedDragImage = dragImageObj.GetComponent<Image>();
            dragImageFound = true;
        }
        else
        {
            Debug.LogWarning("DragImage를 찾을 수 없습니다! 'DragImage' 태그가 설정되어 있는지 확인하세요.");
        }
    }

    /// <summary>
    /// 자식 Transform에서 태그로 GameObject 찾기
    /// </summary>
    private static GameObject FindDragImageInChildren(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
                return child.gameObject;
            
            GameObject found = FindDragImageInChildren(child, tag);
            if (found != null)
                return found;
        }
        return null;
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

        // 같은 타입의 슬롯들 간 교환 (인벤토리 <-> 인벤토리 또는 퀵슬롯 <-> 퀵슬롯)
        if (fromSlot.Type == toSlot.Type)
        {
            if (fromSlot.Type == Slot.SlotType.Inventory && 
                fromSlot.ownerInventory != null && toSlot.ownerInventory != null && 
                fromSlot.ownerInventory == toSlot.ownerInventory)
            {
                // 인벤토리 내부 교환
                fromSlot.ownerInventory.SwapItems(fromSlot.Index, toSlot.Index);
                Debug.Log($"인벤토리 내부 교환: {fromSlot.Index} <-> {toSlot.Index}");
                return;
            }
            
            if (fromSlot.Type == Slot.SlotType.QuickSlot && 
                fromSlot.ownerQuickSlots != null && toSlot.ownerQuickSlots != null &&
                fromSlot.ownerQuickSlots == toSlot.ownerQuickSlots)
            {
                // 퀵슬롯 내부 교환
                fromSlot.ownerQuickSlots.SwapItems(fromSlot.Index, toSlot.Index);
                Debug.Log($"퀵슬롯 내부 교환: {fromSlot.Index} <-> {toSlot.Index}");
                return;
            }
        }

        // 서로 다른 시스템 간 교환 (인벤토리 <-> 퀵슬롯)
        // 퀵슬롯은 인벤토리의 링크/참조 역할
        if ((fromSlot.Type == Slot.SlotType.Inventory && toSlot.Type == Slot.SlotType.QuickSlot) ||
            (fromSlot.Type == Slot.SlotType.QuickSlot && toSlot.Type == Slot.SlotType.Inventory))
        {
            // 인벤토리 -> 퀵슬롯
            if (fromSlot.Type == Slot.SlotType.Inventory && toSlot.Type == Slot.SlotType.QuickSlot)
            {
                if (fromSlot.ownerInventory == null || toSlot.ownerQuickSlots == null)
                {
                    Debug.LogWarning($"인벤토리 -> 퀵슬롯: 소유자가 없습니다. from={fromSlot.ownerInventory}, to={toSlot.ownerQuickSlots}");
                    return;
                }
                
                // 이미 등록된 아이템인지 확인
                if (toSlot.ownerQuickSlots.IsItemRegistered(fromItem))
                {
                    Debug.LogWarning($"인벤토리 -> 퀵슬롯: '{fromItem?.itemName}' 아이템은 이미 퀵슬롯에 등록되어 있습니다.");
                    return;
                }
                
                // 기존 퀵슬롯 아이템 처리
                if (toItem != null)
                {
                    // 퀵슬롯에 있던 아이템은 그냥 클리어
                    toSlot.ownerQuickSlots.items[toSlot.Index] = null;
                }
                // 퀵슬롯에 인벤토리 아이템 링크 설정
                toSlot.ownerQuickSlots.items[toSlot.Index] = fromItem;
                toSlot.ownerQuickSlots.RefreshSlots();
                
                // 퀵슬롯 수량을 인벤토리 수량과 동기화
                toSlot.ItemQuantity = fromSlot.ItemQuantity;
                
                Debug.Log($"인벤토리 -> 퀵슬롯: '{fromItem?.itemName}' 등록 (슬롯 {toSlot.Index}, 수량: {fromSlot.ItemQuantity})");
                return;
            }
            // 퀵슬롯 -> 인벤토리
            else if (fromSlot.Type == Slot.SlotType.QuickSlot && toSlot.Type == Slot.SlotType.Inventory)
            {
                if (fromSlot.ownerQuickSlots == null || toSlot.ownerInventory == null)
                {
                    Debug.LogWarning($"퀵슬롯 -> 인벤토리: 소유자가 없습니다. from={fromSlot.ownerQuickSlots}, to={toSlot.ownerInventory}");
                    return;
                }
                
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
                Debug.Log($"퀵슬롯 -> 인벤토리: '{fromItem?.itemName}' 등록 해제 (슬롯 {fromSlot.Index})");
                return;
            }
        }

        Debug.LogWarning($"슬롯 교환 실패: 호환되지 않는 슬롯 타입. from={fromSlot.Type}, to={toSlot.Type}");
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
