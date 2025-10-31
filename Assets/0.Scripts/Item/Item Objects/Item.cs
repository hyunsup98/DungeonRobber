using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public enum ItemType  // ������ ����
    {
        Equipment,
        Consumable,
        Sellable,
        ETC,
    }

    [Header("������ �̸�")]
    public string itemName; // �������� �̸�
    
    [TextArea]
    public string description;
    
    public ItemType itemType; // ������ ����
    
    [Header("������ �̹���")]
    public Sprite itemImage; // �������� �̹���(�κ��丮 �ȿ��� ��� �̹���)
    
    public GameObject itemPrefab;  // �������� ������ (������ ������ ���������� ��)

    [Header("������ ��� �׼�")]
    public ItemAction useAction;

    public string weaponType;  // ���� ����
}