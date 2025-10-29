using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [Header("������ �̸�")]
    public string itemName;
    
    [TextArea]
    public string description;

    [Header("������ �̹���")]
    public Sprite itemImage;
    
    [Header("������ ��� �׼�")]
    public ItemAction useAction;
}