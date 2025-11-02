using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상자 슬롯 컨텍스트 메뉴 (우클릭 메뉴)
/// </summary>
public class ChestContextMenu : MonoBehaviour
{
    public static ChestContextMenu Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private RectTransform menuRoot; // 메뉴 루트(RectTransform)
    [SerializeField] private Button takeButton;

    private Canvas parentCanvas;
    private RectTransform canvasRect;

    private Slot currentSlot;
    private Item currentItem;
    private Chest currentChest;

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

        if (takeButton != null) takeButton.onClick.AddListener(OnTake);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (takeButton != null) takeButton.onClick.RemoveListener(OnTake);
    }

    public static ChestContextMenu GetOrFind()
    {
        if (Instance == null)
            Instance = FindObjectOfType<ChestContextMenu>();
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

        currentChest = slot.GetComponentInParent<Chest>();

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
        currentChest = null;
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

    private void OnTake()
    {
        if (currentItem == null) return;
        
        if (currentChest != null)
        {
            currentChest.TakeItem(currentItem, 1);
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

