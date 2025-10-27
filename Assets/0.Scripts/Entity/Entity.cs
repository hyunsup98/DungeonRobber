using UnityEngine;

/// <summary>
/// 플레이어, 몬스터 등에서 공통적으로 필요한 스탯, 메서드들을 정의하는 추상 클래스
/// </summary>
public abstract class Entity : MonoBehaviour
{
    protected float attackDamage;
    protected float maxHP;
    protected float currentHP;
    protected float moveSpeed;
    protected float attackRange;
    protected float attackDelay;

    //초기화 메서드
    protected abstract void Init();

    //공격 메서드
    protected abstract void Attack();

    //피격 메서드
    protected abstract void GetDamage(float damage);
}
