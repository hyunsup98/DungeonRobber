using System.Collections.Generic;

/// <summary>
/// 이동 속도를 증가시키는 버프
/// </summary>
public class SpeedBoostBuff : BaseBuff
{
    public override void OnActivate(List<BaseBuff> list, BaseStat stat)
    {
        // 현재 이동속도에 비율만큼 곱해서 증가
        float currentSpeed = stat.GetStat(StatType.MoveSpeed);
        float boostAmount = currentSpeed * (amount / 100f); // amount가 30이면 30% 증가
        stat.ModifyStat(StatType.MoveSpeed, boostAmount);
        
        base.OnActivate(list, stat);
    }

    public override void OnDeActivate(List<BaseBuff> list, BaseStat stat)
    {
        // 원래 속도로 복원 (isRestoreValue가 true면 자동으로 되돌아감)
        base.OnDeActivate(list, stat);
    }
}

