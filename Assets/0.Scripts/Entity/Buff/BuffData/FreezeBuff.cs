using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ȿ�� �� �̵� �ӵ��� �������� �ϴ� ȿ��
/// </summary>
public class FreezeBuff : BaseBuff
{
    public override void OnActivate(List<BaseBuff> list, BaseStat stat)
    {
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
