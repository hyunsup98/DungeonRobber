using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 컨텍스트 메뉴 (우클릭 메뉴)
/// </summary>
public class InventoryContextMenu : MonoBehaviour
{
    public static InventoryContextMenu Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private RectTransform menuRoot; // 메뉴 루트(RectTransform)
    [SerializeField] private Button useButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button addToQuickButton;
    [SerializeField] private Button discardButton;

    [SerializeField] private QuickSlot_Controller quickSlots;
    [SerializeField] private Shop shop;

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
        if (equipButton != null) equipButton.onClick.AddListener(OnEquip);
        if (sellButton != null) sellButton.onClick.AddListener(OnSell);
        if (addToQuickButton != null) addToQuickButton.onClick.AddListener(OnAddToQuick);
        if (discardButton != null) discardButton.onClick.AddListener(OnDiscard);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (useButton != null) useButton.onClick.RemoveListener(OnUse);
        if (equipButton != null) equipButton.onClick.RemoveListener(OnEquip);
        if (sellButton != null) sellButton.onClick.RemoveListener(OnSell);
        if (addToQuickButton != null) addToQuickButton.onClick.RemoveListener(OnAddToQuick);
        if (discardButton != null) discardButton.onClick.RemoveListener(OnDiscard);
    }

    public static InventoryContextMenu GetOrFind()
    {
        if (Instance == null)
            Instance = FindObjectOfType<InventoryContextMenu>();
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
        if (shop == null)
            shop = FindObjectOfType<Shop>();

        // 버튼 표시/숨김 처리
        UpdateButtonVisibility();

        if (menuRoot != null)
        {
            // 자기 자신을 활성화 먼저 (가끔 메뉴가 열리지 않는 경우가 있어서 추가... 더 나은 보완 방식을 찾으면 수정 예정)
            if (!this.gameObject.activeSelf)
                this.gameObject.SetActive(true);
            
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
            if (equipButton != null) equipButton.gameObject.SetActive(false);
            if (sellButton != null) sellButton.gameObject.SetActive(false);
            if (addToQuickButton != null) addToQuickButton.gameObject.SetActive(false);
            if (discardButton != null) discardButton.gameObject.SetActive(false);
            return;
        }

        // 사용: Consumable 타입만
        if (useButton != null)
            useButton.gameObject.SetActive(currentItem.itemType == Item.ItemType.Consumable);

        // 장비: Weapon, Equipment 타입만
        if (equipButton != null)
            equipButton.gameObject.SetActive(currentItem.itemType == Item.ItemType.Weapon || currentItem.itemType == Item.ItemType.Equipment);

        // 판매: 상점 UI가 열려있을 때만
        if (sellButton != null)
        {
            if (shop == null) shop = FindObjectOfType<Shop>();
            sellButton.gameObject.SetActive(shop != null && shop.IsOpen);
        }

        // 퀵슬롯: Consumable 타입이고 이미 등록되지 않은 아이템만
        if (addToQuickButton != null)
        {
            if (quickSlots == null) quickSlots = FindObjectOfType<QuickSlot_Controller>();
            bool canAddToQuick = currentItem.itemType == Item.ItemType.Consumable && 
                                 quickSlots != null && 
                                 !quickSlots.IsItemRegistered(currentItem);
            addToQuickButton.gameObject.SetActive(canAddToQuick);
        }

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
        if (equipButton != null && equipButton.gameObject.activeSelf) activeButtonCount++;
        if (sellButton != null && sellButton.gameObject.activeSelf) activeButtonCount++;
        if (addToQuickButton != null && addToQuickButton.gameObject.activeSelf) activeButtonCount++;
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
            // Item_Controller 찾아서 버프 적용
            Item_Controller itemController = FindObjectOfType<Item_Controller>();
            
            if (itemController != null)
            {
                itemController.ApplyItemBuff(currentItem.useBuff);
                Debug.Log($"'{currentItem.itemName}' 버프 효과가 적용되었습니다!");
            }
            else
            {
                Debug.LogWarning("Item_Controller를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.Log($"'{currentItem.itemName}' 아이템을 사용했습니다! (버프 효과 없음)");
        }
        
        // 소비 아이템은 개수 감소 또는 제거
        if (currentInventory != null && currentItem != null)
        {
            currentInventory.DecreaseItemQuantity(currentItem);
        }
        Hide();
    }

    private void OnEquip()
    {
        if (currentItem == null) return;
        
        // TODO: 장비 슬롯에 장착 (장비 시스템 구현 필요)
        Debug.Log($"'{currentItem.itemName}' 장비를 장착했습니다!");
        Hide();
    }

    private void OnSell()
    {
        if (currentItem == null) return;
        
        if (shop == null) shop = FindObjectOfType<Shop>();
        if (shop == null) return;

        // 컨텍스트 메뉴 닫기 전에 값 저장
        Item itemToSell = currentItem;
        uint maxQuantity = currentSlot?.ItemQuantity ?? 1;
        Inventory invToUse = currentInventory;
        Shop shopToUse = shop;

        // 컨텍스트 메뉴 닫기
        Hide();

        // 수량 선택 다이얼로그 표시
        var dialog = SellQuantityDialog.GetOrFind();
        if (dialog != null)
        {
            dialog.Show(itemToSell, maxQuantity, Input.mousePosition, (quantity) => OnSellConfirmed(quantity, itemToSell, shopToUse, invToUse));
        }
        else
        {
            Debug.LogWarning("SellQuantityDialog를 찾을 수 없습니다!");
        }
    }

    private void OnSellConfirmed(int quantity, Item item, Shop shopRef, Inventory invRef)
    {
        if (item == null || shopRef == null) return;
        
        shopRef.SellItem(item, quantity);
        Debug.Log($"'{item.itemName}' {quantity}개 판매 완료!");
    }

    private void OnAddToQuick()
    {
        if (currentItem == null) return;
        if (quickSlots == null) quickSlots = FindObjectOfType<QuickSlot_Controller>();

        quickSlots?.AddItem(currentItem);
        Hide();
    }

    private void OnDiscard()
    {
        if (currentItem == null) return;
        currentInventory?.RemoveItem(currentSlot.Index);
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
