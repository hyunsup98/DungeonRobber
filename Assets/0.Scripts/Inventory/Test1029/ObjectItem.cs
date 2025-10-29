using UnityEngine;

public interface IObjectItem
{
    ItemModel ClickItem();
}

public class ObjectItem : MonoBehaviour, IObjectItem
{
    [Header("아이템")]
    public ItemModel item;
    [Header("아이템 이미지")]
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