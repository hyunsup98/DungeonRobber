using System.Collections.Generic;

/// <summary>
/// �� ��ƼƼ �� �������� �������� ���۽����� Ŭ����
/// </summary>
public class BuffManager
{
    //���� �������� ���� ����Ʈ �� ���� ������ ������ �� Ȯ�� �� ���Ÿ� ���ֱ� ����
    private List<BaseBuff> buffs = new List<BaseBuff>();

    /// <summary>
    /// ������ ��������ִ� �޼���
    /// </summary>
    /// <param name="buff"></param>
    public void ApplyBuff(BaseBuff buff, BaseStat stat)
    {
        BaseBuff baseBuff;

        if(buffs.Exists(b => b.name == buff.name))
        {
            //�̹� ���� ������ ������ ������
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
