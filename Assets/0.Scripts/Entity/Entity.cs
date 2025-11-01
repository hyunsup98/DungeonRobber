using UnityEngine;

/// <summary>
/// 플레이어, 몬스터 등에서 공통적으로 필요한 스탯, 메서드들을 정의하는 추상 클래스
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected BaseStat stats = new BaseStat();
    protected BuffManager buffManager = new BuffManager();

    /// <summary>
    /// 초기화 메서드
    /// 엔티티가 생성, 스폰될 때 주로 호출
    /// maxHP의 값만 정해져있고 currenHP의 값이 maxHP보다 작거나 재사용을 통해 0인 경우를 방지
    /// 파생 클래스에서 base 키워드를 통해 호출이 가능하고 초기화 기능 확장 가능
    /// </summary>
    protected virtual void Init()
    {
        stats.Init();
    }

    //공격 메서드
    protected abstract void Attack();

    //피격 메서드
    public abstract void GetDamage(float damage);

    //버프 적용 메서드
    public void ApplyBuffToEntity(params BaseBuff[] buffs)
    {
        foreach (var buff in buffs)
        {
            buffManager.ApplyBuff(buff, stats);
        }
    }
}
