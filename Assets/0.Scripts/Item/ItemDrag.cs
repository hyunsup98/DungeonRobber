using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Inventory;

public class ItemDrag : MonoBehaviour
{

    public Transform dragImage;   // 빈 이미지 객체.

    private Image EmptyImg; // 빈 이미지.
    private Slot slot;      // 현재 슬롯에 스크립트

    void Start()
    {
        // 현재 슬롯의 스크립트를 가져온다.
        slot = GetComponent<Slot>();
        // 빈 이미지 객체를 태그를 이용하여 가져온다.
        dragImage = GameObject.FindGameObjectWithTag("DragImage").transform;
        // 빈 이미지 객체가 가진 Image컴포넌트를 가져온다.
        EmptyImg = dragImage.GetComponent<Image>();
    }

    public void Down()
    {
        // 슬롯에 아이템이 없으면 함수종료.
        if (slot.Item == null)
            return;

        // 아이템 사용시.
        //if (Input.GetMouseButtonDown(1))
        //{
        //    slot.ItemUse();
        //    return;
        //}

        // 빈 이미지 객체를 활성화 시킨다.
        dragImage.gameObject.SetActive(true);

        // 빈 이미지의 사이즈를 변경한다.(해상도가 바뀔경우를 대비.)
        float Size = slot.transform.GetComponent<RectTransform>().sizeDelta.x;
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size);
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size);

        // 빈 이미지의 스프라이트를 슬롯의 스프라이트로 변경한다.
        EmptyImg.sprite = slot.Item.itemImage;
        // 빈 이미지의 위치를 마우스위로 가져온다.
        dragImage.transform.position = Input.mousePosition;
        // 슬롯의 아이템 이미지를 없애준다.
        Image slotImage = slot.GetComponent<Image>();
        slotImage.sprite = null;
        slotImage.color = new Color(slotImage.color.r, slotImage.color.g, slotImage.color.b, 0f);
        // 슬롯의 텍스트 숫자를 없애준다.
        //slot.text.text = "";
    }

    public void Drag()
    {
        // isImg플래그가 false이면 슬롯에 아이템이 존재하지 않는 것이므로 함수 종료.
        if (slot.Item == null)
            return;

        dragImage.transform.position = Input.mousePosition;
    }

    public void DragEnd()
    {
        Debug.Log("드래그 종료 함수 실행");
        // isImg플래그가 false이면 슬롯에 아이템이 존재하지 않는 것이므로 함수 종료.
        if (slot.Item == null)
            return;

        Swap(slot, dragImage.transform.position);
        //slot = null;
    }

    public void Up()
    {
        // isImg플래그가 false이면 슬롯에 아이템이 존재하지 않는 것이므로 함수 종료.
        if (slot.Item == null)
            return;

        // 빈 이미지 객체 비활성화.
        dragImage.gameObject.SetActive(false);
        // 슬롯의 아이템 이미지를 복구 시킨다.
        //slot.UpdateInfo(true, slot.slot.Peek().DefaultImg);
        Image slotImage = slot.GetComponent<Image>();
        slotImage.sprite = EmptyImg.sprite;
        slotImage.color = new Color(slotImage.color.r, slotImage.color.g, slotImage.color.b, 1f);
        Debug.Log("아이템 드래그 종료");
    }

    // 계획(의사코드):
    // 1) UI 요소(Graphic) 아래의 슬롯을 먼저 찾는다:
    //    - EventSystem.current와 PointerEventData를 사용해 GraphicRaycaster로 Raycast 실행
    //    - RaycastResult 목록에서 Slot 컴포넌트를 찾기 (GetComponentInParent로 유연하게 탐색)
    //    - 찾으면 슬롯 교체 수행 후 종료
    // 2) UI에서 못찾으면 기존 Physics.Raycast(3D 콜라이더)로 폴백
    // 3) 디버그 로그를 추가하여 어떤 경로로 찾았는지 확인
    private void Swap(Slot slot, Vector2 position)
    {
        Debug.Log("Swap 함수 실행");

        // UI GraphicRaycaster를 통한 Raycast
        if (EventSystem.current != null)
        {
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = position;

            List<RaycastResult> results = new List<RaycastResult>();

            // 우선 dragImage의 부모 캔버스에서 GraphicRaycaster를 찾고, 없으면 씬 전체에서 찾는다.
            GraphicRaycaster gr = null;
            if (dragImage != null)
                gr = dragImage.GetComponentInParent<GraphicRaycaster>();
            if (gr == null)
                gr = FindObjectOfType<GraphicRaycaster>();

            if (gr != null)
            {
                gr.Raycast(ped, results);
                Debug.Log($"UI Raycast 결과 수: {results.Count}");
                foreach (var res in results)
                {
                    Debug.Log($"Raycast hit UI: {res.gameObject.name}");
                    Slot targetSlot = res.gameObject.GetComponent<Slot>() ?? res.gameObject.GetComponentInParent<Slot>();
                    if (targetSlot != null && targetSlot != slot)
                    {
                        Debug.Log($"UI 슬롯 발견 및 스왑: {targetSlot.name}");
                        //Inventory inventory = slot.ownerInventory;
                        //QuickSlot_Controller quickSlots = slot.ownerQuickSlots;
                        //Inventory targetInventory = targetSlot.ownerInventory;
                        //QuickSlot_Controller targetQuickSlots = targetSlot.ownerQuickSlots;

                        //SlotType from = inventory != null ? SlotType.Inventory : SlotType.QuickSlots;
                        //SlotType to = targetInventory != null ? SlotType.Inventory : SlotType.QuickSlots;

                        //// 인벤토리 태그로 인벤토리 찾기
                        //Inventory generalInventory = GameObject.FindWithTag("Inventory").GetComponent<Inventory>();
                        //generalInventory.SwapItem(from, slot.Index, to, targetSlot.Index);
                        return;
                    }
                }
            }
            else
            {
                Debug.LogWarning("GraphicRaycaster를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("EventSystem.current가 null 입니다. UI Raycast 불가.");
        }
    }
}
