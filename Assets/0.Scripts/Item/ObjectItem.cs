using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public interface IObjectItem
{
    Item ClickItem();
}

public class ObjectItem : MonoBehaviour, IObjectItem
{
    [Header("������")]
    public Item item;
    [Header("������ �̹���")]
    public SpriteRenderer itemImage;

    [Header("���̶���Ʈ ����")]
    [Tooltip("ȣ�� �� ǥ�õ� ����")]
    public Color highlightColor = new Color(1f, 0.9f, 0.2f, 0.9f);
    [Tooltip("���� ��������Ʈ ��� ������ ����")]
    public float highlightScale = 1.12f;

    // ��Ÿ������ �����Ǵ� ���̶���Ʈ ������Ʈ
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
    //    // ������ �ϴ� ��Ȱ��ȭ (�κ��̳� �����Կ��� �� �� ����)
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
    //    // ������ �ϴ� ��Ȱ��ȭ (�κ��̳� �����Կ��� �� �� ����)
    //    //ItemTooltip.Instance?.Hide();
    //    if (_highlightObject != null) _highlightObject.SetActive(false);
    //}

    ///// <summary>
    ///// ���̶���Ʈ�� GameObject�� ����(�� ����)
    ///// ���� ��������Ʈ�� ������ �ణ ũ�� ���̵��� �Ͽ� �ܰ� �׵θ� ȿ���� ���ϴ�.
    ///// </summary>
    //private void CreateHighlight()
    //{
    //    if (itemImage == null || itemImage.sprite == null) return;

    //    // �̹� ������ ����
    //    if (_highlightObject != null) return;

    //    _highlightObject = new GameObject("Highlight");
    //    _highlightObject.transform.SetParent(transform, false);
    //    _highlightObject.transform.localPosition = Vector3.zero;
    //    _highlightObject.transform.localRotation = Quaternion.identity;
    //    _highlightObject.transform.localScale = Vector3.one * highlightScale;

    //    _highlightRenderer = _highlightObject.AddComponent<SpriteRenderer>();
    //    _highlightRenderer.sprite = itemImage.sprite;
    //    _highlightRenderer.color = highlightColor;

    //    // ���� ���� ���̾ ���, �������� �ڿ� �׷����� �� (Ȯ��Ǿ� �����ڸ��� �巯��)
    //    _highlightRenderer.sortingLayerID = itemImage.sortingLayerID;
    //    _highlightRenderer.sortingOrder = itemImage.sortingOrder - 1;
    //    //_highlightRenderer.sortingOrder = itemImage.sortingOrder + 1;

    //    // ���̶���Ʈ�� �⺻ ��Ȱ��
    //    _highlightObject.SetActive(false);

    //    // ������ ���� ��Ƽ������ ���� ����� �� �ϰ�������, Ư���� ���̴��� ������ ����� �޶��� �� ����
    //    if (itemImage.sharedMaterial != null)
    //        _highlightRenderer.sharedMaterial = itemImage.sharedMaterial;
    //}
}