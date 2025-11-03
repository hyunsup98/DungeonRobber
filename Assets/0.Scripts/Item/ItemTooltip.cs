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
    private bool isShowing = false;

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

        // Canvas 찾기 (먼저 찾아야 EnsureCanvasParent에서 사용 가능)
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
            parentCanvas = FindObjectOfType<Canvas>();
        if (parentCanvas != null)
            canvasRect = parentCanvas.GetComponent<RectTransform>();
        
        // 자동으로 UI 요소 찾기
        if (tooltipRoot == null)
            tooltipRoot = GetComponent<RectTransform>();
        
        // RectTransform 설정 확인 및 보정 (Canvas를 부모로 설정)
        if (tooltipRoot != null)
        {
            ConfigureRectTransform(tooltipRoot);
        }
        
        // 초기 상태는 비활성화
        if (tooltipRoot != null)
            tooltipRoot.gameObject.SetActive(false);
        
        if (itemNameText == null)
            itemNameText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    /// <summary>
    /// 툴팁 RectTransform을 올바르게 설정합니다.
    /// 추천 설정: Anchor (0, 1), Pivot (0, 1) - 왼쪽 상단 기준
    /// </summary>
    private void ConfigureRectTransform(RectTransform rectTransform)
    {
        if (rectTransform == null) return;
        
        // Canvas를 부모로 설정하여 레이아웃 클램핑 방지
        EnsureCanvasParent(rectTransform);
        
        // 레이아웃 컴포넌트가 부모에 있어도 영향을 받지 않도록 설정
        CanvasGroup canvasGroup = rectTransform.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.ignoreParentGroups = true;
    }
    
    /// <summary>
    /// 툴팁이 Canvas의 직접 자식인지 확인하고, 아니면 이동시킵니다.
    /// 이렇게 하면 인벤토리나 다른 UI 요소의 레이아웃에 클램핑되지 않습니다.
    /// </summary>
    private void EnsureCanvasParent(RectTransform rectTransform)
    {
        if (rectTransform == null) return;
        
        // Canvas 찾기
        Canvas canvas = parentCanvas != null ? parentCanvas : GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        
        if (canvas == null || rectTransform.parent == canvas.transform)
        {
            return; // 이미 Canvas 자식이거나 Canvas를 찾을 수 없음
        }
        
        // Canvas의 RectTransform이 부모가 아니면 Canvas로 이동
        if (rectTransform.parent != canvas.transform)
        {
            // 부모 레이아웃 컴포넌트의 영향을 받지 않도록 Canvas의 직접 자식으로 이동
            rectTransform.SetParent(canvas.transform, false);
            
            // 가장 앞에 표시되도록 마지막 자식으로 이동
            rectTransform.SetAsLastSibling();
        }
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
    /// <param name="position">사용하지 않음 (호환성을 위해 유지)</param>
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

        // 부모 보장 (Canvas로 이동하여 레이아웃 클램핑 방지)
        EnsureCanvasParent(tooltipRoot);

        // 툴팁 표시 (위치는 고정, Unity 에디터에서 설정한 위치 사용)
        tooltipRoot.gameObject.SetActive(true);
        isShowing = true;
    }

    /// <summary>
    /// 툴팁을 숨깁니다.
    /// </summary>
    public void Hide()
    {
        if (tooltipRoot != null)
            tooltipRoot.gameObject.SetActive(false);
        isShowing = false;
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

