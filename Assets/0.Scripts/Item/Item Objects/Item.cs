using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public enum ItemType  // 아이템 유형
    {
        Equipment,
        Consumable,
        Sellable,
        ETC,
    }

    [Header("아이템 이름")]
    public string itemName; // 아이템의 이름
    
    [TextArea]
    public string description;
    
    public ItemType itemType; // 아이템 유형
    
    [Header("아이템 이미지")]
    public Sprite itemImage; // 아이템의 이미지(인벤토리 안에서 띄울 이미지)
    
    public GameObject itemPrefab;  // 아이템의 프리팹 (아이템 생성시 프리팹으로 찍어냄)

    [Header("아이템 사용 액션")]
    public ItemAction useAction;

    public string weaponType;  // 무기 유형
}