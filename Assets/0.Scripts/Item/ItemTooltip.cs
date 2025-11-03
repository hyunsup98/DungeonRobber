using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 아이템 툴팁 UI - 마우스 오버 시 아이템 정보 표시
/// </summary>
public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance { get; private set; }

    [Header("UI 참조")]
    [SerializeField] private RectTransform tooltipRoot;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image itemImage;

    private Canvas parentCanvas;
    private RectTransform canvasRect;

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        if (Instance == null)
            Instance = this;

        // 초기 상태는 비활성화
        if (tooltipRoot != null)
            tooltipRoot.gameObject.SetActive(false);
        
        // Canvas 찾기
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
            canvasRect = parentCanvas.GetComponent<RectTransform>();
        
        // 자동으로 UI 요소 찾기
        if (tooltipRoot == null)
            tooltipRoot = GetComponent<RectTransform>();
        
        if (itemNameText == null)
            itemNameText = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// ItemTooltip 인스턴스를 찾거나 생성합니다.
    /// </summary>
    public static ItemTooltip GetOrFind()
    {
        if (Instance != null)
            return Instance;
        
        // 씬에서 찾기
        Instance = FindObjectOfType<ItemTooltip>();
        
        if (Instance == null)
        {
            Debug.LogWarning("ItemTooltip: 씬에서 ItemTooltip을 찾을 수 없습니다. Canvas에 ItemTooltip 컴포넌트를 추가해주세요.");
        }
        
        return Instance;
    }

    /// <summary>
    /// 아이템 정보를 표시합니다.
    /// </summary>
    /// <param name="item">표시할 아이템</param>
    /// <param name="position">툴팁 위치 (스크린 좌표)</param>
    public void Show(Item item, Vector2 position)
    {
        if (item == null)
        {
            Hide();
            return;
        }

        if (tooltipRoot == null)
        {
            Debug.LogWarning("ItemTooltip: tooltipRoot가 설정되지 않았습니다.");
            return;
        }

        // 아이템 정보 업데이트
        if (itemNameText != null)
            itemNameText.text = item.itemName ?? "이름 없음";
        
        if (itemTypeText != null)
            itemTypeText.text = GetItemTypeText(item.itemType);
        
        if (descriptionText != null)
            descriptionText.text = item.description ?? "";
        
        if (priceText != null)
            priceText.text = $"구매: {item.buyPrice}G  판매: {item.sellPrice}G";
        
        if (itemImage != null)
        {
            if (item.itemImage != null)
            {
                itemImage.sprite = item.itemImage;
                itemImage.color = Color.white;
            }
            else
            {
                itemImage.sprite = null;
                itemImage.color = new Color(1, 1, 1, 0);
            }
        }

        // 툴팁 위치 설정
        SetTooltipPosition(position);

        // 툴팁 표시
        tooltipRoot.gameObject.SetActive(true);
    }

    /// <summary>
    /// 툴팁을 숨깁니다.
    /// </summary>
    public void Hide()
    {
        if (tooltipRoot != null)
            tooltipRoot.gameObject.SetActive(false);
    }

    /// <summary>
    /// 툴팁 위치를 설정합니다. (스크린 좌표를 RectTransform 로컬 좌표로 변환)
    /// </summary>
    private void SetTooltipPosition(Vector2 screenPosition)
    {
        if (tooltipRoot == null || canvasRect == null || parentCanvas == null)
            return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
            out localPoint
        );

        // 마우스 위치에 표시하되, 화면 밖으로 나가지 않도록 조정
        Vector2 offset = new Vector2(10, -10); // 마우스에서 약간 오프셋
        localPoint += offset;

        // 화면 경계 체크 및 조정
        Rect tooltipRect = tooltipRoot.rect;
        Rect canvasRectBounds = canvasRect.rect;

        if (localPoint.x + tooltipRect.width > canvasRectBounds.xMax)
            localPoint.x = canvasRectBounds.xMax - tooltipRect.width;
        
        if (localPoint.y - tooltipRect.height < canvasRectBounds.yMin)
            localPoint.y = canvasRectBounds.yMin + tooltipRect.height;

        tooltipRoot.anchoredPosition = localPoint;
    }

    /// <summary>
    /// 아이템 타입을 한글 텍스트로 변환합니다.
    /// </summary>
    private string GetItemTypeText(Item.ItemType type)
    {
        switch (type)
        {
            case Item.ItemType.Weapon:
                return "무기";
            case Item.ItemType.Equipment:
                return "장비";
            case Item.ItemType.Consumable:
                return "소비 아이템";
            case Item.ItemType.Sellable:
                return "판매 아이템";
            case Item.ItemType.ETC:
                return "기타";
            default:
                return "알 수 없음";
        }
    }
}

