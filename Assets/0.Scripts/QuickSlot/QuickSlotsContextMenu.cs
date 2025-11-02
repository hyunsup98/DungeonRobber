using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퀵 슬롯 컨텍스트 메뉴 (우클릭 메뉴)
/// </summary>
public class QuickSlotsContextMenu : MonoBehaviour
{
    public static QuickSlotsContextMenu Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private RectTransform menuRoot; // 메뉴 루트(RectTransform)
    [SerializeField] private Button useButton;
    [SerializeField] private Button removeFromQuickButton;
    [SerializeField] private Button discardButton;

    [Tooltip("옵션: 에디터에서 할당하면 자동으로 찾을 필요가 없습니다. 할당하지 않으면 씬에서 Find합니다.")]
    [SerializeField] private QuickSlot_Controller quickSlots;

    private Canvas parentCanvas;
    private RectTransform canvasRect;

    private Slot currentSlot;
    private Item currentItem;
    private Inventory currentInventory;

    void Awake()
    {
        // Instance가 null이 아니지만 자기 자신인 경우 (GetOrFind에서 이미 설정됨)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Instance가 null이면 자기 자신으로 설정
        if (Instance == null)
            Instance = this;

        if (menuRoot != null) menuRoot.gameObject.SetActive(false);
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null) canvasRect = parentCanvas.GetComponent<RectTransform>();

        if (useButton != null) useButton.onClick.AddListener(OnUse);
        if (removeFromQuickButton != null) removeFromQuickButton.onClick.AddListener(OnRemoveFromQuick);
        if (discardButton != null) discardButton.onClick.AddListener(OnDiscard);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (useButton != null) useButton.onClick.RemoveListener(OnUse);
        if (removeFromQuickButton != null) removeFromQuickButton.onClick.RemoveListener(OnRemoveFromQuick);
        if (discardButton != null) discardButton.onClick.RemoveListener(OnDiscard);
    }

    public static QuickSlotsContextMenu GetOrFind()
    {
        if (Instance == null)
            Instance = FindObjectOfType<QuickSlotsContextMenu>();
        return Instance;
    }

    public void Show(Slot slot, Vector2 screenPosition)
    {
        currentSlot = slot;
        currentItem = slot != null ? slot.Item : null;
        if (currentItem == null)
        {
            Hide();
            return;
        }

        currentInventory = slot.ownerInventory ?? slot.GetComponentInParent<Inventory>();
        if (quickSlots == null)
            quickSlots = FindObjectOfType<QuickSlot_Controller>();
        
        // 퀵슬롯인 경우 Inventory 참조 가져오기
        if (currentInventory == null && quickSlots != null)
            currentInventory = quickSlots.inventory;

        // 버튼 표시/숨김 처리
        UpdateButtonVisibility();

        if (menuRoot != null)
        {
            menuRoot.gameObject.SetActive(true);
            SetPosition(screenPosition);
        }
    }

    public void Hide()
    {
        if (menuRoot != null) menuRoot.gameObject.SetActive(false);
        currentSlot = null;
        currentItem = null;
        currentInventory = null;
    }

    private void SetPosition(Vector2 screenPosition)
    {
        if (canvasRect == null || menuRoot == null) return;
        Camera cam = parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? parentCanvas.worldCamera : null;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, cam, out localPoint);
        menuRoot.localPosition = localPoint;
        ClampToCanvas();
    }

    private void ClampToCanvas()
    {
        if (canvasRect == null || menuRoot == null) return;

        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);
        Vector3[] menuCorners = new Vector3[4];
        menuRoot.GetWorldCorners(menuCorners);

        Vector3 delta = Vector3.zero;
        if (menuCorners[2].x > canvasCorners[2].x) delta.x = canvasCorners[2].x - menuCorners[2].x;
        if (menuCorners[0].x < canvasCorners[0].x) delta.x = canvasCorners[0].x - menuCorners[0].x;
        if (menuCorners[2].y > canvasCorners[2].y) delta.y = canvasCorners[2].y - menuCorners[2].y;
        if (menuCorners[0].y < canvasCorners[0].y) delta.y = canvasCorners[0].y - menuCorners[0].y;

        menuRoot.position += delta;
    }

    /// <summary>
    /// 버튼 표시/숨김 처리
    /// </summary>
    private void UpdateButtonVisibility()
    {
        if (currentItem == null)
        {
            if (useButton != null) useButton.gameObject.SetActive(false);
            if (removeFromQuickButton != null) removeFromQuickButton.gameObject.SetActive(false);
            if (discardButton != null) discardButton.gameObject.SetActive(false);
            return;
        }

        // 사용: Consumable 타입만
        if (useButton != null)
            useButton.gameObject.SetActive(currentItem.itemType == Item.ItemType.Consumable);

        // 등록해제: 항상 표시
        if (removeFromQuickButton != null)
            removeFromQuickButton.gameObject.SetActive(true);

        // 버리기: 항상 표시
        if (discardButton != null)
            discardButton.gameObject.SetActive(true);
        
        // 높이 자동 조정
        AdjustMenuHeight();
    }
    
    /// <summary>
    /// 활성화된 버튼 개수에 맞춰 메뉴 높이 자동 조정
    /// </summary>
    private void AdjustMenuHeight()
    {
        if (menuRoot == null) return;
        
        // 활성화된 버튼 개수 계산
        int activeButtonCount = 0;
        if (useButton != null && useButton.gameObject.activeSelf) activeButtonCount++;
        if (removeFromQuickButton != null && removeFromQuickButton.gameObject.activeSelf) activeButtonCount++;
        if (discardButton != null && discardButton.gameObject.activeSelf) activeButtonCount++;
        
        // 버튼이 없으면 기본 높이로
        if (activeButtonCount == 0)
        {
            activeButtonCount = 1; // 최소 높이 보장
        }
        
        // VerticalLayoutGroup이 있는지 확인하여 버튼 높이 + 간격 계산
        UnityEngine.UI.VerticalLayoutGroup layoutGroup = menuRoot.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        float buttonHeight = layoutGroup != null ? layoutGroup.preferredHeight : 50f; // 기본값 50
        float spacing = layoutGroup != null ? layoutGroup.spacing : 10f; // 기본값 10
        float padding = layoutGroup != null ? layoutGroup.padding.top + layoutGroup.padding.bottom : 40f; // 기본값 40
        
        // 총 높이 = (버튼 개수 * 버튼 높이) + ((버튼 개수 - 1) * 간격) + 패딩
        float totalHeight = (activeButtonCount * buttonHeight) + ((activeButtonCount - 1) * spacing) + padding;
        
        // 메뉴 높이 적용
        menuRoot.sizeDelta = new Vector2(menuRoot.sizeDelta.x, totalHeight);
    }

    private void OnUse()
    {
        if (currentItem == null) return;
        
        // 아이템 버프 적용
        if (currentItem.useBuff != null)
        {
            // Player_Controller 찾아서 버프 적용
            Player_Controller player = Player_Controller.Instance;
            if (player == null)
                player = FindObjectOfType<Player_Controller>();
            
            if (player != null)
            {
                player.ApplyItemBuff(currentItem.useBuff);
                Debug.Log($"'{currentItem.itemName}' 버프 효과가 적용되었습니다!");
            }
            else
            {
                Debug.LogWarning("Player_Controller를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.Log($"'{currentItem.itemName}' 아이템을 사용했습니다! (버프 효과 없음)");
        }
        
        // 사용 후 소비
        // 인벤토리 수량 감소
        if (currentInventory != null && currentItem != null)
        {
            currentInventory.DecreaseItemQuantity(currentItem);
            
            // 인벤토리 수량 확인
            uint remainingQuantity = currentInventory.GetItemQuantity(currentItem);
            
            if (remainingQuantity == 0)
            {
                // 수량이 0이 되면 퀵슬롯에서도 제거
                quickSlots?.RemoveItem(currentSlot.Index);
            }
            else
            {
                // 수량이 남아있으면 퀵슬롯 수량 동기화
                quickSlots?.RefreshSlots();
            }
        }
        Hide();
    }

    private void OnRemoveFromQuick()
    {
        if (currentItem == null) return;
        if (quickSlots == null) quickSlots = FindObjectOfType<QuickSlot_Controller>();

        // 퀵슬롯에서 제거 (인벤토리는 유지)
        quickSlots?.RemoveItem(currentSlot.Index);
        Hide();
    }

    private void OnDiscard()
    {
        if (currentItem == null) return;
        
        // 퀵슬롯에서 제거
        if (quickSlots == null) quickSlots = FindObjectOfType<QuickSlot_Controller>();
        quickSlots?.RemoveItem(currentSlot.Index);
        
        // 인벤토리에서도 제거
        if (currentInventory != null)
        {
            currentInventory.RemoveItem(currentItem);
        }
        
        Hide();
    }

    void Update()
    {
        if (menuRoot != null && menuRoot.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 메뉴 밖 클릭 시 닫기
                Camera cam = parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? parentCanvas.worldCamera : null;
                if (!RectTransformUtility.RectangleContainsScreenPoint(menuRoot, Input.mousePosition, cam))
                    Hide();
            }
        }
    }
}
