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

    private float lastClickTime = 0f;
    private const float doubleClickTime = 0.3f; // 더블클릭 감지 시간 (초)

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (_item == null) return;

        Shop shop = GetComponentInParent<Shop>();
        if (shop == null) return;

        // 좌클릭: 아이템 구매
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 더블클릭 감지
            float currentTime = Time.time;
            if (currentTime - lastClickTime < doubleClickTime)
            {
                // 더블클릭 - 즉시 구매
                shop.TryBuyItem(_item, 1);
                lastClickTime = 0f; // 더블클릭 후 리셋
            }
            else
            {
                // 단일 클릭 - 첫 클릭만 기록 (UI 피드백용)
                lastClickTime = currentTime;
            }
        }
        // 우클릭: 아이템 구매 (기존 기능 유지)
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            shop.TryBuyItem(_item, 1);
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

