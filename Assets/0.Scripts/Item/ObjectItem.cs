using UnityEngine;

public interface IObjectItem
{
    ItemModel ClickItem();
}

public class ObjectItem : MonoBehaviour, IObjectItem
{
    [Header("������")]
    public ItemModel item;
    [Header("������ �̹���")]
    public SpriteRenderer itemImage;

    void Start()
    {
        itemImage.sprite = item.itemImage;
    }
    public ItemModel ClickItem()
    {
        Debug.Log($"Click Item: {item.itemName}");
        return this.item;
    }
}