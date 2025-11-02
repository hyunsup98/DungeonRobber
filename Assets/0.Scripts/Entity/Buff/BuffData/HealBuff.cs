using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체력을 회복하는 버프 (즉시 회복)
/// </summary>
public class HealBuff : BaseBuff
{
    public override void OnActivate(List<BaseBuff> list, BaseStat stat)
    {
        // 즉시 체력 회복
        stat.ModifyStat(StatType.HP, amount);
        Debug.Log($"체력이 {amount} 회복되었습니다!");
        
        // 버프는 즉시 종료 (지속시간 없음)
        OnDeActivate(list, stat);
    }

    public override void OnTick(BaseStat stat)
    {
        // 즉시 효과이므로 Tick 없음
    }

    public override void OnDeActivate(List<BaseBuff> list, BaseStat stat)
    {
        base.OnDeActivate(list, stat);
    }
}

