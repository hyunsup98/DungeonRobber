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
public abstract class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image image;

    protected enum SlotType
    {
        Inventory,
        QuickSlot
    }
    [SerializeField] protected SlotType slotType;

    // 슬롯 소유자(인벤토리 / 퀵슬롯)와 인덱스 정보
    public Inventory ownerInventory;

    public Image defaultImage;

    // 슬롯 자체 인덱스(읽기전용)
    public int Index { get; private set; } = -1;
    public uint ItemQuantity { get => _itemQuantity; set => _itemQuantity = value; }


    // 내부 필드
    protected private Item _item;
    private uint _itemQuantity = 0;

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


    private void Awake()
    {
        UpdateIndexFromHierarchy();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateIndexFromHierarchy();
    }
#endif

    /// <summary>
    /// 부모(또는 슬롯 컨테이너)의 GetComponentsInChildren 순서와 동일한 인덱스를 계산하여 Index에 저장합니다.
    /// </summary>
    private void UpdateIndexFromHierarchy()
    {
        // 부모가 없으면 -1
        if (transform.parent == null)
        {
            Index = -1;
            return;
        }

        // 슬롯 컨테이너(부모)
        var slotContainer = transform.parent;

        // 슬롯 컨테이너의 부모(=grandparent)가 있으면 grandparent 위에서 검색,
        // 없으면 부모(slotContainer) 위에서 검색
        Transform searchRoot = slotContainer.parent != null ? slotContainer.parent : slotContainer;

        // null 체크(안정성)
        if (searchRoot == null)
        {
            Index = -1;
            return;
        }

        var allSlots = searchRoot.GetComponentsInChildren<Slot>();
        for (int i = 0; i < allSlots.Length; i++)
        {
            if (allSlots[i] == this)
            {
                Index = i;
                return;
            }
        }

        // 못 찾았을 경우
        Index = -1;
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

    public abstract void OnPointerClick(PointerEventData eventData);
    
        // 우클릭 처리: 인벤토리에서 컨텍스트 메뉴 표시
        //if (eventData.button == PointerEventData.InputButton.Right)
        //{
        //    Debug.Log($"_item: {_item}");
        //    if (_item == null) return;


        //    // 퀵슬롯일 경우
        //    if (ownerQuickSlots != null)
        //    {
        //        var menu = QuickSlotsContextMenu.GetOrFind();
        //        if (menu != null)
        //        {
        //            menu.Show(this, eventData.position);
        //        }
        //    }
        //    else
        //    {
        //        var menu = InventoryContextMenu.GetOrFind();
        //        if (menu != null)
        //        {
        //            menu.Show(this, eventData.position);
        //        }
        //    }
        //}
    

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