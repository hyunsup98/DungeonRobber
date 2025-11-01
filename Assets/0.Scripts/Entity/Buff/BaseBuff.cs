using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 다양한 효과에 관련된 기능을 수행하는 클래스,
/// 이로운 효과뿐만 아니라, 이동 속도 감소, 출혈딜 등의 해로운 효과도 포함
/// </summary>
public abstract class BaseBuff : MonoBehaviour
{
    [Header("증감시킬 스탯 타입")]
    [SerializeField] protected StatType statType;           //어떤 타입의 스탯을 조정할건지

    [Header("증감시킬 양")]
    [SerializeField] protected float amount;                //얼마만큼의 수치를 바꿀건지

    [Header("지속시간(초)")]
    [SerializeField] protected float duration;              //몇 초 동안 지속될 효과인지

    [Header("주기마다 계속 발동시킬 것인지")]
    [SerializeField] protected bool isTick;                 //한 번이 아닌 특정 주기마다 계속 실행되는 효과인지

    [Header("몇 초 주기로 발동시킬 것인지")]
    [SerializeField] protected float tickTime;              //몇 초마다 실행될건지

    [Header("효과가 끝나면 원래 값을 다시 되돌릴 것인지")]
    [SerializeField] protected bool isRestoreValue;         //효과가 끝날 때 다시 원래 값을 되돌릴 것인지

    private float restoreValue = 0f;                        //효과가 끝난 후 수치를 되돌릴 경우 저장해놓을 기존 수치값

    private Coroutine buffCoroutine;

    //버프가 활성화되었을 때 호출할 메서드
    public virtual void OnActivate(List<BaseBuff> list, BaseStat stat)
    {
        //이미 버프 효과가 실행되고 있는 경우 → 중첩되지 않으므로 중지
        if (buffCoroutine != null)
        {
            StopCoroutine(buffCoroutine);

            //다시 수치를 되돌려야 하는 버프의 경우 다시 되돌림 → 중첩되지 않도록
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

    //버프가 활성화되어 있는 중 → isTick이 True일 경우 반복문을 통해 주기적 수행
    public IEnumerator Activating(List<BaseBuff> list, BaseStat stat)
    {
        if(isTick)
        {
            //isTick이 true면 duration 타임동안 주기적으로(tickSeconds마다) OnTick메서드 호출
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
            //isTick이 false면 duration만큼 기다림
            OnTick(stat);
            yield return CoroutineManager.waitForSeconds(duration);
        }

        OnDeActivate(list, stat);
    }

    public virtual void OnTick(BaseStat stat)
    {
        stat.ModifyStat(statType, amount);
    }

    //효과가 끝날 때 호출할 메서드
    public virtual void OnDeActivate(List<BaseBuff> list, BaseStat stat)
    {
        //리스트에 현재 버프 객체가 존재하면 지워줌 → 할 일 끝
        if (list != null && list.Exists(b => b == this))
        {
            list.Remove(this);
        }

        if(isRestoreValue)
        {
            stat.SetStat(statType, restoreValue);
        }

        //풀 안에 집어넣기
        BuffPool.Instance.TakeObjects(this);
    }
}
