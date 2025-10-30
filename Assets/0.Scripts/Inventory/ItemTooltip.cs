using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 상의 아이템 툴팁을 제어합니다.
/// </summary>
public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance { get; private set; }

    [SerializeField] private RectTransform tooltipRoot;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Vector2 pivotOffset = new Vector2(15f, -15f);

    private RectTransform canvasRect;
    private Canvas parentCanvas;
    private CanvasGroup tooltipCanvasGroup;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) { Destroy(gameObject); return; }

        Debug.Log("[ItemTooltip] Awake - instance set");

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null) canvasRect = parentCanvas.GetComponent<RectTransform>();

        if (tooltipRoot == null) tooltipRoot = GetComponent<RectTransform>();
        if (tooltipRoot != null)
        {
            tooltipRoot.gameObject.SetActive(false);

            // 툴팁이 포인터 이벤트를 가로채지 않도록 설정 (깜빡임 방지)
            tooltipCanvasGroup = tooltipRoot.GetComponent<CanvasGroup>();
            if (tooltipCanvasGroup == null) tooltipCanvasGroup = tooltipRoot.gameObject.AddComponent<CanvasGroup>();
            tooltipCanvasGroup.blocksRaycasts = false;
            tooltipCanvasGroup.interactable = false;

            // 추가 안전장치: 내부 Graphic들의 raycastTarget 끄기
            var graphics = tooltipRoot.GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics) g.raycastTarget = false;
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // 안전한 접근용(Instance가 없으면 씬에서 찾아서 반환)
    public static ItemTooltip GetOrFind()
    {
        if (Instance == null)
            Instance = FindObjectOfType<ItemTooltip>(true);
        return Instance;
    }

    void Update()
    {
        if (tooltipRoot != null && tooltipRoot.gameObject.activeSelf)
        {
            SetPosition(Input.mousePosition);
        }
    }

    public void Show(Item item)
    {
        if (item == null) return;
        if (nameText != null) nameText.text = item.itemName;
        if (descriptionText != null) descriptionText.text = item.description;
        if (tooltipRoot != null) tooltipRoot.gameObject.SetActive(true);
        SetPosition(Input.mousePosition);
    }

    public void Hide()
    {
        if (tooltipRoot != null) tooltipRoot.gameObject.SetActive(false);
    }

    public void SetPosition(Vector2 screenPosition)
    {
        if (canvasRect == null || tooltipRoot == null) return;

        Camera cam = parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera
            ? parentCanvas.worldCamera : null;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, cam, out localPoint);
        tooltipRoot.localPosition = localPoint + pivotOffset;
        
        // 화면 밖으로 나가지 않게 클램프
        ClampToCanvas();
    }

    private void ClampToCanvas()
    {
        if (canvasRect == null || tooltipRoot == null) return;

        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);
        Vector3[] tooltipCorners = new Vector3[4];
        tooltipRoot.GetWorldCorners(tooltipCorners);

        Vector3 delta = Vector3.zero;
        if (tooltipCorners[2].x > canvasCorners[2].x) delta.x = canvasCorners[2].x - tooltipCorners[2].x;
        if (tooltipCorners[0].x < canvasCorners[0].x) delta.x = canvasCorners[0].x - tooltipCorners[0].x;
        if (tooltipCorners[2].y > canvasCorners[2].y) delta.y = canvasCorners[2].y - tooltipCorners[2].y;
        if (tooltipCorners[0].y < canvasCorners[0].y) delta.y = canvasCorners[0].y - tooltipCorners[0].y;

        tooltipRoot.position += delta;
    }
}