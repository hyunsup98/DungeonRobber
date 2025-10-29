using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image slotItemImage;
    [SerializeField] private Image EventImage;
    [SerializeField] private TMP_Text slotItemCountText;
    private Item item;
    private bool isIn = false;

    public Item Item { get => item; set => item = value; }
    public bool IsIn { get => isIn; set => isIn = value; }

    void Awake()
    {
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        foreach (var cvs in canvases)
        {
            if (cvs.CompareTag("Inventory"))
            {
                canvas = cvs;
                break;
            }
        }
        draggingPlane = canvas.transform as RectTransform;
        slotItemCountText.text = "";
    }

    public void SetSlot(Item setItem, bool isNewItem)
    {
        if (setItem != null)
        {
            GetItem(setItem);
            if (isNewItem)
            {
                setItem.Count = 1;
            }
            SlotUpdate(setItem);
        }
        UpdateCountText();
    }

    private void GetItem(Item getItem)
    {
        // 아이템 획득 처리
    }

    private void SlotUpdate(Item updateItem)
    {
        Debug.Log($"Slot Update Called: {updateItem}");
        Debug.Log($"Slot Update Called: {updateItem.Name}");
        SetColor(1f, slotItemImage);
        this.slotItemImage.sprite = updateItem.GetComponent<Image>().sprite;
        this.IsIn = true;
        this.Item = updateItem;
        Debug.Log($"Slot Update: {Item.Name}");
    }

    private void ClearSlot()
    {
        SetColor(0f, slotItemImage);
        this.slotItemImage.sprite = null;
        this.IsIn = false;
        this.Item = null;
        UpdateCountText();
    }

    public void UpdateCountText()
    {
        if (Item != null && Item.Count > 0)
        {
            slotItemCountText.text = Item.Count.ToString();
        }
        else
        {
            slotItemCountText.text = "";
        }
    }

    public void AddCount(int count)
    {
        if (Item != null)
        {
            Item.Count += count;
            UpdateCountText();
        }
    }

    private void SetColor(float alpha, Image setImage)
    {
        Color color = setImage.color;
        color.a = alpha;
        setImage.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetColor(1f, EventImage);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetColor(0f, EventImage);
    }



    private Canvas canvas;
    private GameObject draggingIcon;
    private RectTransform draggingPlane;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Item == null)
            return;
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;
        // 드래그 아이콘 생성
        draggingIcon = new GameObject("Dragging Icon");
        draggingIcon.transform.SetParent(canvas.transform, false);
        draggingIcon.transform.SetAsLastSibling();
        Image image = draggingIcon.AddComponent<Image>();
        image.sprite = slotItemImage.sprite;
        image.SetNativeSize();
        image.raycastTarget = false;
        SetColor(0.6f, image);
        draggingPlane = canvas.transform as RectTransform;
        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingIcon != null)
        {
            SetDraggedPosition(eventData);
        }
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        RectTransform rt = draggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = draggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingIcon != null)
        {
            Destroy(draggingIcon);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Slot droppedSlot = eventData.pointerDrag.GetComponent<Slot>();
        if (droppedSlot != null)
        {
            Item droppedItem = droppedSlot.Item;
            Item thisItem = this.Item;
            // 아이템 교환
            this.SlotUpdate(droppedItem);
            droppedSlot.SlotUpdate(thisItem);
        }
    }

    public void SetSlotItem(Item setItem) { Item = setItem; }
    public void SetSlotImage(Image slotImage) { slotItemImage = slotImage; }
    public void SetCountText(int count) { slotItemCountText.text = count.ToString(); }
    
    public Item GetItem() { return Item; }
}

