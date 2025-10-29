using UnityEngine;

[CreateAssetMenu]
public class ItemModel : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite itemImage;
    public ItemAction useAction; // NULL이면 사용 불가
}