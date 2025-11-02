using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        Weapon,      // 무기 (장비 슬롯에만 등록)
        Equipment,   // 장비 (장비 슬롯에 등록 가능)
        Consumable,  // 소비 아이템 (사용 가능)
        Sellable,    // 판매 가능한 아이템
        ETC,         // 기타
    }

    [Header("아이템 정보")]
    public string itemName;
    
    [TextArea]
    public string description;
    
    public ItemType itemType;
    
    [Header("가격 정보")]
    public int buyPrice = 100;   // 구매 가격
    public int sellPrice = 50;   // 판매 가격
    
    [Header("아이템 이미지")]
    public Sprite itemImage;
    
    public GameObject itemPrefab;

    public string weaponType;
}
