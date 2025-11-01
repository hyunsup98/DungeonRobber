using System.Collections.Generic;

/// <summary>
/// 각 엔티티 별 실질적인 버프들을 동작시켜줄 클래스
/// </summary>
public class BuffManager
{
    //현재 적용중인 버프 리스트 → 같은 버프가 들어왔을 때 확인 후 갱신만 해주기 위함
    private List<BaseBuff> buffs = new List<BaseBuff>();

    /// <summary>
    /// 버프를 적용시켜주는 메서드
    /// </summary>
    /// <param name="buff"></param>
    public void ApplyBuff(BaseBuff buff, BaseStat stat)
    {
        BaseBuff baseBuff;

        if(buffs.Exists(b => b.name == buff.name))
        {
            //이미 같은 형식의 버프가 있으면
            baseBuff = buffs.Find(b => b.name == buff.name);
        }
        else
        {
            baseBuff = BuffPool.Instance.GetObjects(buff, BuffPool.Instance.transform);
            buffs.Add(baseBuff);
        }

        baseBuff.OnActivate(buffs, stat);
    }
}
