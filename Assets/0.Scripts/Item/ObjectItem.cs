using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public interface IObjectItem
{
    Item ClickItem();
}

public class ObjectItem : MonoBehaviour, IObjectItem
{
    [Header("아이템")]
    public Item item;
    [Header("아이템 이미지")]
    public SpriteRenderer itemImage;

    [Header("하이라이트 설정")]
    [Tooltip("호버 시 표시될 색상")]
    public Color highlightColor = new Color(1f, 0.9f, 0.2f, 0.9f);
    [Tooltip("원본 스프라이트 대비 스케일 비율")]
    public float highlightScale = 1.12f;

    // 런타임으로 생성되는 하이라이트 오브젝트
    private GameObject _highlightObject;
    private SpriteRenderer _highlightRenderer;

    void Start()
    {
        if (itemImage != null)
            itemImage.sprite = item.itemImage;

        //CreateHighlight();
    }

    public Item ClickItem()
    {
        Debug.Log($"Click Item: {item.itemName}");
        return this.item;
    }

    //void OnMouseEnter()
    //{
    //    Debug.Log($"OnMouseEnter {gameObject.name}");
    //    // 툴팁은 일단 비활성화 (인벤이나 퀵슬롯에서 볼 수 있음)
    //    //if (item != null && ItemTooltip.Instance != null)
    //    //{
    //    //    ItemTooltip.Instance.Show(item);
    //    //    ItemTooltip.Instance.SetPosition(Camera.main.WorldToScreenPoint(transform.position));
    //    //}
    //    if (_highlightObject != null) _highlightObject.SetActive(true);
    //}

    //void OnMouseExit()
    //{
    //    Debug.Log($"OnMouseExit {gameObject.name}");
    //    // 툴팁은 일단 비활성화 (인벤이나 퀵슬롯에서 볼 수 있음)
    //    //ItemTooltip.Instance?.Hide();
    //    if (_highlightObject != null) _highlightObject.SetActive(false);
    //}

    ///// <summary>
    ///// 하이라이트용 GameObject를 생성(한 번만)
    ///// 원본 스프라이트를 복제해 약간 크게 보이도록 하여 외곽 테두리 효과를 냅니다.
    ///// </summary>
    //private void CreateHighlight()
    //{
    //    if (itemImage == null || itemImage.sprite == null) return;

    //    // 이미 있으면 재사용
    //    if (_highlightObject != null) return;

    //    _highlightObject = new GameObject("Highlight");
    //    _highlightObject.transform.SetParent(transform, false);
    //    _highlightObject.transform.localPosition = Vector3.zero;
    //    _highlightObject.transform.localRotation = Quaternion.identity;
    //    _highlightObject.transform.localScale = Vector3.one * highlightScale;

    //    _highlightRenderer = _highlightObject.AddComponent<SpriteRenderer>();
    //    _highlightRenderer.sprite = itemImage.sprite;
    //    _highlightRenderer.color = highlightColor;

    //    // 같은 정렬 레이어를 사용, 원본보다 뒤에 그려지게 함 (확대되어 가장자리가 드러남)
    //    _highlightRenderer.sortingLayerID = itemImage.sortingLayerID;
    //    _highlightRenderer.sortingOrder = itemImage.sortingOrder - 1;
    //    //_highlightRenderer.sortingOrder = itemImage.sortingOrder + 1;

    //    // 하이라이트는 기본 비활성
    //    _highlightObject.SetActive(false);

    //    // 원본과 같은 머티리얼을 쓰면 모양이 더 일관되지만, 특별한 셰이더가 있으면 결과가 달라질 수 있음
    //    if (itemImage.sharedMaterial != null)
    //        _highlightRenderer.sharedMaterial = itemImage.sharedMaterial;
    //}
}