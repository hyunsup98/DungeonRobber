using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ȭ�� ���� ������ ������ �����մϴ�.
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

            // ������ ������ �̺�Ʈ�� ����ä�� �ʵ��� ���� (������ ����)
            tooltipCanvasGroup = tooltipRoot.GetComponent<CanvasGroup>();
            if (tooltipCanvasGroup == null) tooltipCanvasGroup = tooltipRoot.gameObject.AddComponent<CanvasGroup>();
            tooltipCanvasGroup.blocksRaycasts = false;
            tooltipCanvasGroup.interactable = false;

            // �߰� ������ġ: ���� Graphic���� raycastTarget ����
            var graphics = tooltipRoot.GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics) g.raycastTarget = false;
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ������ ���ٿ�(Instance�� ������ ������ ã�Ƽ� ��ȯ)
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
        
        // ȭ�� ������ ������ �ʰ� Ŭ����
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