using UnityEngine;

public abstract class ItemAction : ScriptableObject
{
    // ȣ�� �� ���� ����ߴ��� ����
    public abstract void Use(ItemModel item, GameObject user);
}