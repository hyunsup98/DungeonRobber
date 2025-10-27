using UnityEngine;

/// <summary>
/// 플레이어, 몬스터 등에서 공통적으로 필요한 스탯, 메서드들을 정의하는 추상 클래스
/// </summary>
public abstract class Entity : MonoBehaviour
{
   [SerializeField] protected float attackDamage;
   [SerializeField] protected float maxHP;
   [SerializeField] protected float currentHP;
   [SerializeField] protected float moveSpeed;
   [SerializeField] protected float attackRange;
   [SerializeField] protected float attackDelay;

    //초기화 메서드
    protected abstract void Init();

    //공격 메서드
    protected abstract void Attack();

    //피격 메서드
    protected abstract void GetDamage(float damage);
}
