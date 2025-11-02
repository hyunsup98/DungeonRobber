using System.Collections.Generic;
using UnityEngine;

#region 스탯의 타입
//스탯의 타입
public enum StatType
{
    HP,             //체력, baseStats에선 maxHP역할 / stats에선 currentHP 역할
    MoveSpeed,      //이동 속도
    AttackDamage,   //공격력
    AttackRange,    //공격 사거리
    AttackDelay     //공격 속도
}
#endregion

#region 각 스탯들이 가질 스탯 정보 클래스
/// <summary>
/// 각각의 스탯들이 가질 스탯의 타입과 수치 정보
/// 잦은 수정을 고려해 strcut가 아닌 class로 작성
/// struct의 경우 깊은 복사로 수정될 때 마다 복사본의 값을 받아와야 함
/// </summary>
[System.Serializable]
public class Stat
{
    [SerializeField] private StatType type;                             //스탯 타입
    public StatType Type => type;
    [field : SerializeField] public float Value { get; private set; }   //스탯 수치

    public Stat(StatType type, float value)
    {
        this.type = type;
        Value = value;
    }

    //수치를 조정하는 메서드
    public void AddValue(float amount)
    {
        if (Value + amount < 0)
            Value = 0;
        else
            Value += amount;
    }

    //수치를 세팅하는 메서드
    public void SetValue(float value)
    {
        if (value < 0)
            Value = 0;
        else
            Value = value;
    }
}
#endregion

[System.Serializable]
public class BaseStat
{
    #region 스탯 초기화 시 참조할 스탯 리스트
    [Header("초기 스탯 값, 버프 등의 효과가 끝나고 초기 수치로 되돌리기 위해 참조할 초기 값")]
    //초기 스탯
    [SerializeField] private List<Stat> baseStats = new List<Stat>();
    #endregion

    [Header("게임 플레이 도중 실시간으로 변할 스탯 값")]
    //게임 플레이 중 실시간으로 변할 스탯
    private List<Stat> stats = new List<Stat>();

    //엔티티 생성 시 초기 세팅 메서드
    public void Init()
    {
        if (baseStats == null) return;

        foreach (var stat in baseStats)
        {
            Stat s = new Stat(stat.Type, stat.Value);
            stats.Add(s);
        }
    }

    //특정 타입의 능력치를 base 값으로 초기화
    public void InitStat(StatType type)
    {
        ModifyStat(type, GetBaseStat(type));
    }

    #region BaseStats 값 관련 메서드
    public float GetBaseStat(StatType type)
    {
        return baseStats.Find(stat => stat.Type == type).Value;
    }

    public void SetBaseStat(StatType type, float value)
    {
        baseStats.Find(stat => stat.Type == type).SetValue(value);
    }

    //레벨업 등의 이유로 baseStats을 수정
    public void ModifyBaseStat(StatType type, float amount)
    {
        baseStats.Find(stat => stat.Type == type).AddValue(amount);
    }
    #endregion

    #region stats 값 관련 메서드
    //엔티티 스탯의 특정 타입 값 반환
    public float GetStat(StatType type)
    {
        return stats.Find(stat => stat.Type == type).Value;
    }

    public void SetStat(StatType type, float value)
    {
        stats.Find(stat => stat.Type == type).SetValue(value);
    }

    //도트딜, 이속 감소 등의 효과로 엔티티 스탯 수정
    public void ModifyStat(StatType type, float amount)
    {
        stats.Find(stat => stat.Type == type).AddValue(amount);
    }
    #endregion
}
