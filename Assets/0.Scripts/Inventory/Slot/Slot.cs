using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ���� ���� UI�� ����մϴ�.
/// private �ʵ�: _item
/// public ������Ƽ: Item (UI ����)
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

    // ���� ������(�κ��丮 / ������)�� �ε��� ����
    public Inventory ownerInventory;

    public Image defaultImage;

    // ���� ��ü �ε���(�б�����)
    public int Index { get; private set; } = -1;
    public uint ItemQuantity { get => _itemQuantity; set => _itemQuantity = value; }


    // ���� �ʵ�
    protected private Item _item;
    private uint _itemQuantity = 0;

    /// <summary>
    /// ���Կ� ���õ� ������. set���� UI �̹��� ���� ó��.
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
    /// �θ�(�Ǵ� ���� �����̳�)�� GetComponentsInChildren ������ ������ �ε����� ����Ͽ� Index�� �����մϴ�.
    /// </summary>
    private void UpdateIndexFromHierarchy()
    {
        // �θ� ������ -1
        if (transform.parent == null)
        {
            Index = -1;
            return;
        }

        // ���� �����̳�(�θ�)
        var slotContainer = transform.parent;

        // ���� �����̳��� �θ�(=grandparent)�� ������ grandparent ������ �˻�,
        // ������ �θ�(slotContainer) ������ �˻�
        Transform searchRoot = slotContainer.parent != null ? slotContainer.parent : slotContainer;

        // null üũ(������)
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

        // �� ã���� ���
        Index = -1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (_item != null && ItemTooltip.Instance != null)
        //{
        //    ItemTooltip.Instance.Show(_item);
        //    ItemTooltip.Instance.SetPosition(eventData.position);
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (ItemTooltip.Instance != null)
        //    ItemTooltip.Instance.Hide();
    }

    public abstract void OnPointerClick(PointerEventData eventData);
    
        // ��Ŭ�� ó��: �κ��丮���� ���ؽ�Ʈ �޴� ǥ��
        //if (eventData.button == PointerEventData.InputButton.Right)
        //{
        //    Debug.Log($"_item: {_item}");
        //    if (_item == null) return;


        //    // �������� ���
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
    

    // ����׿� ������ �ٿ�/�� �̺�Ʈ
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log($"OnPointerDown on {gameObject.name}  pressObject: {eventData.pointerPress?.name}");
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    Debug.Log($"OnPointerUp on {gameObject.name}  currentRaycast: {eventData.pointerCurrentRaycast.gameObject?.name}");
    //}
}