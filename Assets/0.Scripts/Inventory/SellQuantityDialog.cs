using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 아이템 판매 수량 입력 다이얼로그
/// </summary>
public class SellQuantityDialog : MonoBehaviour
{
    public static SellQuantityDialog Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private RectTransform dialogRoot;
    [SerializeField] private TMP_InputField quantityInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Canvas parentCanvas;
    private RectTransform canvasRect;

    private System.Action<int> onConfirm;
    private Item currentItem;
    private uint maxQuantity;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (dialogRoot != null) dialogRoot.gameObject.SetActive(false);
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null) canvasRect = parentCanvas.GetComponent<RectTransform>();

        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirm);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancel);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (confirmButton != null) confirmButton.onClick.RemoveListener(OnConfirm);
        if (cancelButton != null) cancelButton.onClick.RemoveListener(OnCancel);
    }

    public static SellQuantityDialog GetOrFind()
    {
        if (Instance == null)
            Instance = FindObjectOfType<SellQuantityDialog>();
        return Instance;
    }

    public void Show(Item item, uint maxQuantity, Vector2 screenPosition, System.Action<int> onConfirmCallback)
    {
        currentItem = item;
        this.maxQuantity = maxQuantity;
        onConfirm = onConfirmCallback;

        // 입력 필드 초기화
        if (quantityInputField != null)
        {
            quantityInputField.text = maxQuantity.ToString();
            quantityInputField.Select();
            quantityInputField.ActivateInputField();
        }

        if (dialogRoot != null)
        {
            dialogRoot.gameObject.SetActive(true);
            SetPosition(screenPosition);
        }
    }

    public void Hide()
    {
        if (dialogRoot != null) dialogRoot.gameObject.SetActive(false);
        currentItem = null;
        onConfirm = null;
        maxQuantity = 0;
    }

    private void SetPosition(Vector2 screenPosition)
    {
        if (canvasRect == null || dialogRoot == null) return;
        Camera cam = parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? parentCanvas.worldCamera : null;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, cam, out localPoint);
        dialogRoot.localPosition = localPoint;
        ClampToCanvas();
    }

    private void ClampToCanvas()
    {
        if (canvasRect == null || dialogRoot == null) return;

        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);
        Vector3[] dialogCorners = new Vector3[4];
        dialogRoot.GetWorldCorners(dialogCorners);

        Vector3 delta = Vector3.zero;
        if (dialogCorners[2].x > canvasCorners[2].x) delta.x = canvasCorners[2].x - dialogCorners[2].x;
        if (dialogCorners[0].x < canvasCorners[0].x) delta.x = canvasCorners[0].x - dialogCorners[0].x;
        if (dialogCorners[2].y > canvasCorners[2].y) delta.y = canvasCorners[2].y - dialogCorners[2].y;
        if (dialogCorners[0].y < canvasCorners[0].y) delta.y = canvasCorners[0].y - dialogCorners[0].y;

        dialogRoot.position += delta;
    }

    private void OnConfirm()
    {
        int quantity = 0;
        
        if (quantityInputField != null && !string.IsNullOrEmpty(quantityInputField.text))
        {
            if (!int.TryParse(quantityInputField.text, out quantity) || quantity <= 0)
            {
                Debug.LogWarning("유효하지 않은 수량입니다.");
                return;
            }
        }
        else
        {
            quantity = 1;
        }

        // 최대 수량 체크
        if (quantity > maxQuantity)
        {
            Debug.LogWarning($"보유 수량({maxQuantity})보다 많습니다.");
            quantity = (int)maxQuantity;
        }

        onConfirm?.Invoke(quantity);
        Hide();
    }

    private void OnCancel()
    {
        Hide();
    }
}

