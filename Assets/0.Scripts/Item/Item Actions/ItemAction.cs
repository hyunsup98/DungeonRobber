using UnityEngine;

public abstract class ItemAction : ScriptableObject
{
    // 호출 시 누가 사용했는지 전달
    public abstract void Use(Item item, GameObject user);
}