using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 모든 슬롯의 기본 클래스
/// private 필드: _item
/// public 프로퍼티: Item (UI 업데이트)
/// </summary>
public abstract class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image image;

    public enum SlotType
    {
        Inventory,
        QuickSlot,
        Equipment,
        Shop,
        Chest
    }
    [SerializeField] protected SlotType slotType;

    // 슬롯 소유자(인벤토리 / 퀵슬롯)의 참조
    public Inventory ownerInventory;
    public QuickSlot_Controller ownerQuickSlots;

    public Image defaultImage;

    // 슬롯의 인덱스(오브젝트별로)
    public int Index { get; private set; } = -1;
    public uint ItemQuantity 
    { 
        get => _itemQuantity; 
        set 
        { 
            _itemQuantity = value; 
            UpdateQuantityText();
        }
    }
    public SlotType Type { get => slotType; }

    // 아이템 필드
    protected Item _item;
    private uint _itemQuantity = 0;
    
    // 수량 표시 UI
    private Text quantityText;

    /// <summary>
    /// 슬롯에 배치된 아이템. set시 UI 이미지 업데이트 처리.
    /// </summary>
    public Item Item
    {
        get { return _item; }
        set
        {
            _item = value;
            
            if (image == null)
                image = GetComponentInChildren<Image>();

            if (image != null)
            {
                if (_item != null && _item.itemImage != null)
                {
                    image.sprite = _item.itemImage;
                    image.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    image.sprite = defaultImage?.sprite;
                    image.color = new Color(1, 1, 1, _item != null ? 1 : 0);
                }
            }
            
            // 수량 텍스트 업데이트
            UpdateQuantityText();
        }
    }

    protected virtual void Awake()
    {
        UpdateIndexFromHierarchy();
        
        // 이미지 컴포넌트 자동 찾기
        if (image == null)
            image = GetComponentInChildren<Image>();
        
        // 수량 텍스트 찾기
        quantityText = GetComponentInChildren<Text>();
        
        // 소유자 자동 찾기
        FindOwner();
    }
    
    /// <summary>
    /// 슬롯 타입에 따라 소유자를 자동으로 찾아서 설정합니다.
    /// </summary>
    private void FindOwner()
    {
        // 항상 부모에서 모두 찾기
        if (ownerInventory == null)
        {
            ownerInventory = GetComponentInParent<Inventory>();
        }
        
        if (ownerQuickSlots == null)
        {
            ownerQuickSlots = GetComponentInParent<QuickSlot_Controller>();
        }
        
        // 둘 다 설정되었으면 slotType에 따라 정리 (둘 중 하나만 남기기)
        if (ownerInventory != null && ownerQuickSlots != null)
        {
            if (slotType == SlotType.Inventory)
                ownerQuickSlots = null;
            else if (slotType == SlotType.QuickSlot)
                ownerInventory = null;
        }
        
        // 부모에서 못 찾으면 씬에서 찾기 (폴백)
        if (ownerInventory == null && slotType == SlotType.Inventory)
        {
            ownerInventory = FindObjectOfType<Inventory>();
        }
        
        if (ownerQuickSlots == null && slotType == SlotType.QuickSlot)
        {
            ownerQuickSlots = FindObjectOfType<QuickSlot_Controller>();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateIndexFromHierarchy();
        
        if (image == null)
            image = GetComponentInChildren<Image>();
        
        // 에디터에서도 소유자 자동 찾기
        if (!Application.isPlaying)
        {
            FindOwner();
        }
    }
#endif

    /// <summary>
    /// 부모(또는 빈 슬롯 컨테이너)로부터 GetComponentsInChildren를 통해 슬롯의 인덱스를 찾아서 Index를 설정합니다.
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

        // 슬롯 컨테이너의 부모(=grandparent)가 있으면 grandparent를 검색,
        // 없으면 부모(slotContainer)를 검색
        Transform searchRoot = slotContainer.parent != null ? slotContainer.parent : slotContainer;

        // null 체크(에러 방지)
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

        // 못 찾은 경우
        Index = -1;
    }

    /// <summary>
    /// 수량 텍스트를 업데이트합니다.
    /// </summary>
    private void UpdateQuantityText()
    {
        if (quantityText == null)
            return;
        
        // 수량이 1 이상일 때만 표시
        if (_itemQuantity > 1)
        {
            quantityText.text = _itemQuantity.ToString();
        }
        else
        {
            quantityText.text = "";
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
    }

    public abstract void OnPointerClick(PointerEventData eventData);
}
