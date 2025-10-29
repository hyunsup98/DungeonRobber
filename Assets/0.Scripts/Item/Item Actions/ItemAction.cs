using UnityEngine;

/// <summary>
/// ������ ��� ������ ĸ��ȭ�ϴ� ScriptableObject �߻� Ŭ����
/// </summary>
public abstract class ItemAction : ScriptableObject
{
    /// <summary>
    /// ������ ��� �� ����Ǵ� �޼���
    /// </summary>
    /// <param name="item">���� ������ ����</param>
    /// <param name="user">����� GameObject(��: Player)</param>
    public abstract void Use(Item item, GameObject user);
}