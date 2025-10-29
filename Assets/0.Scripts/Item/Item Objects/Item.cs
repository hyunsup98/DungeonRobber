using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [Header("아이템 이름")]
    public string itemName;
    
    [TextArea]
    public string description;

    [Header("아이템 이미지")]
    public Sprite itemImage;
    
    [Header("아이템 사용 액션")]
    public ItemAction useAction;
}