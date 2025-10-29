using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public interface IObjectItem
{
    Item ClickItem();
}

public class ObjectItem : MonoBehaviour, IObjectItem, IPointerEnterHandler, IPointerExitHandler
{
    [Header("아이템")]
    public Item item;
    [Header("아이템 이미지")]
    public SpriteRenderer itemImage;

    void Start()
    {
        itemImage.sprite = item.itemImage;
    }
    public Item ClickItem()
    {
        Debug.Log($"Click Item: {item.itemName}");
        return this.item;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.item != null && ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.Show(this.item);
            ItemTooltip.Instance.SetPosition(Camera.main.WorldToScreenPoint(transform.position));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemTooltip.Instance != null)
            ItemTooltip.Instance.Hide();
    }
}