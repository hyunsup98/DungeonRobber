using UnityEngine;

public abstract class ItemAction : ScriptableObject
{
    // ȣ�� �� ���� ����ߴ��� ����
    public abstract void Use(Item item, GameObject user);
}