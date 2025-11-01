using System.Collections.Generic;
using UnityEngine;

#region ������ Ÿ��
//������ Ÿ��
public enum StatType
{
    HP,             //ü��, baseStats���� maxHP���� / stats���� currentHP ����
    MoveSpeed,      //�̵� �ӵ�
    AttackDamage,   //���ݷ�
    AttackRange,    //���� ��Ÿ�
    AttackDelay     //���� �ӵ�
}
#endregion

#region �� ���ȵ��� ���� ���� ���� Ŭ����
/// <summary>
/// ������ ���ȵ��� ���� ������ Ÿ�԰� ��ġ ����
/// ���� ������ ����� strcut�� �ƴ� class�� �ۼ�
/// struct�� ��� ���� ����� ������ �� ���� ���纻�� ���� �޾ƿ;� ��
/// </summary>
[System.Serializable]
public class Stat
{
    [SerializeField] private StatType type;                             //���� Ÿ��
    public StatType Type => type;
    [field : SerializeField] public float Value { get; private set; }   //���� ��ġ

    public Stat(StatType type, float value)
    {
        this.type = type;
        Value = value;
    }

    //��ġ�� �����ϴ� �޼���
    public void AddValue(float amount)
    {
        if (Value + amount < 0)
            Value = 0;
        else
            Value += amount;
    }

    //��ġ�� �����ϴ� �޼���
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
    #region ���� �ʱ�ȭ �� ������ ���� ����Ʈ
    [Header("�ʱ� ���� ��, ���� ���� ȿ���� ������ �ʱ� ��ġ�� �ǵ����� ���� ������ �ʱ� ��")]
    //�ʱ� ����
    [SerializeField] private List<Stat> baseStats = new List<Stat>();
    #endregion

    [Header("���� �÷��� ���� �ǽð����� ���� ���� ��")]
    //���� �÷��� �� �ǽð����� ���� ����
    private List<Stat> stats = new List<Stat>();

    //��ƼƼ ���� �� �ʱ� ���� �޼���
    public void Init()
    {
        if (baseStats == null) return;

        foreach (var stat in baseStats)
        {
            Stat s = new Stat(stat.Type, stat.Value);
            stats.Add(s);
        }
    }

    //Ư�� Ÿ���� �ɷ�ġ�� base ������ �ʱ�ȭ
    public void InitStat(StatType type)
    {
        ModifyStat(type, GetBaseStat(type));
    }

    #region BaseStats �� ���� �޼���
    public float GetBaseStat(StatType type)
    {
        return baseStats.Find(stat => stat.Type == type).Value;
    }

    public void SetBaseStat(StatType type, float value)
    {
        baseStats.Find(stat => stat.Type == type).SetValue(value);
    }

    //������ ���� ������ baseStats�� ����
    public void ModifyBaseStat(StatType type, float amount)
    {
        baseStats.Find(stat => stat.Type == type).AddValue(amount);
    }
    #endregion

    #region stats �� ���� �޼���
    //��ƼƼ ������ Ư�� Ÿ�� �� ��ȯ
    public float GetStat(StatType type)
    {
        return stats.Find(stat => stat.Type == type).Value;
    }

    public void SetStat(StatType type, float value)
    {
        stats.Find(stat => stat.Type == type).SetValue(value);
    }

    //��Ʈ��, �̼� ���� ���� ȿ���� ��ƼƼ ���� ����
    public void ModifyStat(StatType type, float amount)
    {
        stats.Find(stat => stat.Type == type).AddValue(amount);
    }
    #endregion
}
