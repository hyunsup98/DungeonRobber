using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 상점 슬롯 - 구매 가격 표시 및 구매 기능
/// </summary>
public class ShopSlot : Slot
{
    [Header("Shop UI")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    
    protected override void Awake()
    {
        slotType = SlotType.Shop; // 상점 슬롯 타입으로 설정
        base.Awake(); // 부모의 Awake 호출 (FindOwner 포함)
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭: 아이템 구매
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_item == null) return;

            // 상점 구매 로직 호출
            Shop shop = GetComponentInParent<Shop>();
            if (shop != null)
            {
                shop.TryBuyItem(_item, 1);
            }
        }
    }

    // 아이템 설정 시 가격 표시 업데이트
    public new Item Item
    {
        get { return _item; }
        set
        {
            base.Item = value;
            UpdatePriceDisplay();
            UpdateItemNameDisplay();
        }
    }

    /// <summary>
    /// 가격 텍스트 업데이트
    /// </summary>
    private void UpdatePriceDisplay()
    {
        if (priceText != null && _item != null)
        {
            priceText.text = $"${_item.buyPrice}";
            priceText.gameObject.SetActive(true);
        }
        else if (priceText != null)
        {
            priceText.text = "";
            priceText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 아이템 이름 표시 업데이트
    /// </summary>
    private void UpdateItemNameDisplay()
    {
        if (itemNameText != null && _item != null)
        {
            itemNameText.text = _item.itemName;
            itemNameText.gameObject.SetActive(true);
        }
        else if (itemNameText != null)
        {
            itemNameText.text = "";
            itemNameText.gameObject.SetActive(false);
        }
    }

    // OnPointerEnter 오버라이드 - 아이템 정보 툴팁 표시
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        // TODO: 아이템 상세 정보 툴팁 표시
        if (_item != null)
        {
            Debug.Log($"[상점] {_item.itemName}: {_item.description}");
        }
    }

    // OnPointerExit 오버라이드
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        // TODO: 툴팁 숨기기
    }
}

