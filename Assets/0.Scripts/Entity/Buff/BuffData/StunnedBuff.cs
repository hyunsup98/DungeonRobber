using System.Collections.Generic;

public class StunnedBuff : BaseBuff
{
     public override void OnActivate(List<BaseBuff> list, BaseStat stat)
    {
        statType = StatType.MoveSpeed;
        amount = stat.GetStat(StatType.MoveSpeed);
        base.OnActivate(list, stat);
    }

    public override void OnTick(BaseStat stat)
    {
        base.OnTick(stat);
    }

    public override void OnDeActivate(List<BaseBuff> list, BaseStat stat)
    {
        base.OnDeActivate(list, stat);
    }
}
