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

    public Transform dragImage;   // �� �̹��� ��ü.

    private Image EmptyImg; // �� �̹���.
    private Slot slot;      // ���� ���Կ� ��ũ��Ʈ

    void Start()
    {
        // ���� ������ ��ũ��Ʈ�� �����´�.
        slot = GetComponent<Slot>();
        // �� �̹��� ��ü�� �±׸� �̿��Ͽ� �����´�.
        dragImage = GameObject.FindGameObjectWithTag("DragImage").transform;
        // �� �̹��� ��ü�� ���� Image������Ʈ�� �����´�.
        EmptyImg = dragImage.GetComponent<Image>();
    }

    public void Down()
    {
        // ���Կ� �������� ������ �Լ�����.
        if (slot.Item == null)
            return;

        // ������ ����.
        //if (Input.GetMouseButtonDown(1))
        //{
        //    slot.ItemUse();
        //    return;
        //}

        // �� �̹��� ��ü�� Ȱ��ȭ ��Ų��.
        dragImage.gameObject.SetActive(true);

        // �� �̹����� ����� �����Ѵ�.(�ػ󵵰� �ٲ��츦 ���.)
        float Size = slot.transform.GetComponent<RectTransform>().sizeDelta.x;
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size);
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size);

        // �� �̹����� ��������Ʈ�� ������ ��������Ʈ�� �����Ѵ�.
        EmptyImg.sprite = slot.Item.itemImage;
        // �� �̹����� ��ġ�� ���콺���� �����´�.
        dragImage.transform.position = Input.mousePosition;
        // ������ ������ �̹����� �����ش�.
        Image slotImage = slot.GetComponent<Image>();
        slotImage.sprite = null;
        slotImage.color = new Color(slotImage.color.r, slotImage.color.g, slotImage.color.b, 0f);
        // ������ �ؽ�Ʈ ���ڸ� �����ش�.
        //slot.text.text = "";
    }

    public void Drag()
    {
        // isImg�÷��װ� false�̸� ���Կ� �������� �������� �ʴ� ���̹Ƿ� �Լ� ����.
        if (slot.Item == null)
            return;

        dragImage.transform.position = Input.mousePosition;
    }

    public void DragEnd()
    {
        Debug.Log("�巡�� ���� �Լ� ����");
        // isImg�÷��װ� false�̸� ���Կ� �������� �������� �ʴ� ���̹Ƿ� �Լ� ����.
        if (slot.Item == null)
            return;

        Swap(slot, dragImage.transform.position);
        //slot = null;
    }

    public void Up()
    {
        // isImg�÷��װ� false�̸� ���Կ� �������� �������� �ʴ� ���̹Ƿ� �Լ� ����.
        if (slot.Item == null)
            return;

        // �� �̹��� ��ü ��Ȱ��ȭ.
        dragImage.gameObject.SetActive(false);
        // ������ ������ �̹����� ���� ��Ų��.
        //slot.UpdateInfo(true, slot.slot.Peek().DefaultImg);
        Image slotImage = slot.GetComponent<Image>();
        slotImage.sprite = EmptyImg.sprite;
        slotImage.color = new Color(slotImage.color.r, slotImage.color.g, slotImage.color.b, 1f);
        Debug.Log("������ �巡�� ����");
    }

    // ��ȹ(�ǻ��ڵ�):
    // 1) UI ���(Graphic) �Ʒ��� ������ ���� ã�´�:
    //    - EventSystem.current�� PointerEventData�� ����� GraphicRaycaster�� Raycast ����
    //    - RaycastResult ��Ͽ��� Slot ������Ʈ�� ã�� (GetComponentInParent�� �����ϰ� Ž��)
    //    - ã���� ���� ��ü ���� �� ����
    // 2) UI���� ��ã���� ���� Physics.Raycast(3D �ݶ��̴�)�� ����
    // 3) ����� �α׸� �߰��Ͽ� � ��η� ã�Ҵ��� Ȯ��
    private void Swap(Slot slot, Vector2 position)
    {
        Debug.Log("Swap �Լ� ����");

        // UI GraphicRaycaster�� ���� Raycast
        if (EventSystem.current != null)
        {
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = position;

            List<RaycastResult> results = new List<RaycastResult>();

            // �켱 dragImage�� �θ� ĵ�������� GraphicRaycaster�� ã��, ������ �� ��ü���� ã�´�.
            GraphicRaycaster gr = null;
            if (dragImage != null)
                gr = dragImage.GetComponentInParent<GraphicRaycaster>();
            if (gr == null)
                gr = FindObjectOfType<GraphicRaycaster>();

            if (gr != null)
            {
                gr.Raycast(ped, results);
                Debug.Log($"UI Raycast ��� ��: {results.Count}");
                foreach (var res in results)
                {
                    Debug.Log($"Raycast hit UI: {res.gameObject.name}");
                    Slot targetSlot = res.gameObject.GetComponent<Slot>() ?? res.gameObject.GetComponentInParent<Slot>();
                    if (targetSlot != null && targetSlot != slot)
                    {
                        Debug.Log($"UI ���� �߰� �� ����: {targetSlot.name}");
                        //Inventory inventory = slot.ownerInventory;
                        //QuickSlot_Controller quickSlots = slot.ownerQuickSlots;
                        //Inventory targetInventory = targetSlot.ownerInventory;
                        //QuickSlot_Controller targetQuickSlots = targetSlot.ownerQuickSlots;

                        //SlotType from = inventory != null ? SlotType.Inventory : SlotType.QuickSlots;
                        //SlotType to = targetInventory != null ? SlotType.Inventory : SlotType.QuickSlots;

                        //// �κ��丮 �±׷� �κ��丮 ã��
                        //Inventory generalInventory = GameObject.FindWithTag("Inventory").GetComponent<Inventory>();
                        //generalInventory.SwapItem(from, slot.Index, to, targetSlot.Index);
                        return;
                    }
                }
            }
            else
            {
                Debug.LogWarning("GraphicRaycaster�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("EventSystem.current�� null �Դϴ�. UI Raycast �Ұ�.");
        }
    }
}
