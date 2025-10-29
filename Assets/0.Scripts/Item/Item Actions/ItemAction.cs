using UnityEngine;

/// <summary>
/// 아이템 사용 동작을 캡슐화하는 ScriptableObject 추상 클래스
/// </summary>
public abstract class ItemAction : ScriptableObject
{
    /// <summary>
    /// 아이템 사용 시 실행되는 메서드
    /// </summary>
    /// <param name="item">사용된 아이템 정보</param>
    /// <param name="user">사용자 GameObject(예: Player)</param>
    public abstract void Use(Item item, GameObject user);
}