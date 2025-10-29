using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContextMenu : MonoBehaviour
{
    public static InventoryContextMenu Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private RectTransform menuRoot; // 메뉴 루트(RectTransform)
    [SerializeField] private Button addToQuickButton;
    [SerializeField] private Button discardButton;

    [Tooltip("옵션: 에디터에서 할당하면 자동으로 사용됩니다. 비어있으면 런타임에 Find합니다.")]
    [SerializeField] private QuickSlots quickSlots;

    private Canvas parentCanvas;
    private RectTransform canvasRect;

    private Slot currentSlot;
    private Item currentItem;
    private Inventory currentInventory;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (menuRoot != null) menuRoot.gameObject.SetActive(false);
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null) canvasRect = parentCanvas.GetComponent<RectTransform>();

        if (addToQuickButton != null) addToQuickButton.onClick.AddListener(OnAddToQuick);
        if (discardButton != null) discardButton.onClick.AddListener(OnDiscard);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
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
        currentItem = slot != null ? slot.item : null;
        if (currentItem == null)
        {
            Hide();
            return;
        }

        currentInventory = slot.ownerInventory ?? slot.GetComponentInParent<Inventory>();
        if (quickSlots == null)
            quickSlots = FindObjectOfType<QuickSlots>();

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

    private void OnAddToQuick()
    {
        if (currentItem == null) return;
        if (quickSlots == null) quickSlots = FindObjectOfType<QuickSlots>();

        quickSlots?.AddItem(currentItem);
        currentInventory?.RemoveItem(currentItem);
        Hide();
    }

    private void OnDiscard()
    {
        if (currentItem == null) return;
        currentInventory?.RemoveItem(currentItem);
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