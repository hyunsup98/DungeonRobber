using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �پ��� ȿ���� ���õ� ����� �����ϴ� Ŭ����,
/// �̷ο� ȿ���Ӹ� �ƴ϶�, �̵� �ӵ� ����, ������ ���� �طο� ȿ���� ����
/// </summary>
public abstract class BaseBuff : MonoBehaviour
{
    [Header("������ų ���� Ÿ��")]
    [SerializeField] protected StatType statType;           //� Ÿ���� ������ �����Ұ���

    [Header("������ų ��")]
    [SerializeField] protected float amount;                //�󸶸�ŭ�� ��ġ�� �ٲܰ���

    [Header("���ӽð�(��)")]
    [SerializeField] protected float duration;              //�� �� ���� ���ӵ� ȿ������

    [Header("�ֱ⸶�� ��� �ߵ���ų ������")]
    [SerializeField] protected bool isTick;                 //�� ���� �ƴ� Ư�� �ֱ⸶�� ��� ����Ǵ� ȿ������

    [Header("�� �� �ֱ�� �ߵ���ų ������")]
    [SerializeField] protected float tickTime;              //�� �ʸ��� ����ɰ���

    [Header("ȿ���� ������ ���� ���� �ٽ� �ǵ��� ������")]
    [SerializeField] protected bool isRestoreValue;         //ȿ���� ���� �� �ٽ� ���� ���� �ǵ��� ������

    private float restoreValue = 0f;                        //ȿ���� ���� �� ��ġ�� �ǵ��� ��� �����س��� ���� ��ġ��

    private Coroutine buffCoroutine;

    //������ Ȱ��ȭ�Ǿ��� �� ȣ���� �޼���
    public virtual void OnActivate(List<BaseBuff> list, BaseStat stat)
    {
        //�̹� ���� ȿ���� ����ǰ� �ִ� ��� �� ��ø���� �����Ƿ� ����
        if (buffCoroutine != null)
        {
            StopCoroutine(buffCoroutine);

            //�ٽ� ��ġ�� �ǵ����� �ϴ� ������ ��� �ٽ� �ǵ��� �� ��ø���� �ʵ���
            if (isRestoreValue)
            {
                stat.SetStat(statType, restoreValue);
            }
        }

        if(isRestoreValue)
        {
            restoreValue = stat.GetStat(statType);
        }

        buffCoroutine = StartCoroutine(Activating(list, stat));
    }

    //������ Ȱ��ȭ�Ǿ� �ִ� �� �� isTick�� True�� ��� �ݺ����� ���� �ֱ��� ����
    public IEnumerator Activating(List<BaseBuff> list, BaseStat stat)
    {
        if(isTick)
        {
            //isTick�� true�� duration Ÿ�ӵ��� �ֱ�������(tickSeconds����) OnTick�޼��� ȣ��
            var timer = duration;

            while (timer > 0)
            {
                OnTick(stat);
                yield return CoroutineManager.waitForSeconds(tickTime);
                timer -= tickTime;
            }
        }
        else
        {
            //isTick�� false�� duration��ŭ ��ٸ�
            OnTick(stat);
            yield return CoroutineManager.waitForSeconds(duration);
        }

        OnDeActivate(list, stat);
    }

    public virtual void OnTick(BaseStat stat)
    {
        stat.ModifyStat(statType, amount);
    }

    //ȿ���� ���� �� ȣ���� �޼���
    public virtual void OnDeActivate(List<BaseBuff> list, BaseStat stat)
    {
        //����Ʈ�� ���� ���� ��ü�� �����ϸ� ������ �� �� �� ��
        if (list != null && list.Exists(b => b == this))
        {
            list.Remove(this);
        }

        if(isRestoreValue)
        {
            stat.SetStat(statType, restoreValue);
        }

        //Ǯ �ȿ� ����ֱ�
        BuffPool.Instance.TakeObjects(this);
    }
}
