using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 hp, 스탯 관련 UI를 보여주는 클래스
/// </summary>
public class UI_PlayerStat : MonoBehaviour
{
    [Header("hpBar 관련 변수들")]
    //hpBar 관련 변수
    [SerializeField] private Image imageHp;                 //hpBar 이미지
    [SerializeField] private TMP_Text textHp;               //hpBar 텍스트 - "현재체력 / 최대체력"
    [SerializeField] private float fadeTime = 0.3f;         //체력이 깎일 때 깎이는 모션이 걸리는 시간
    private Coroutine hpCoroutine;

    [Header("스탯 관련 변수들")]
    [SerializeField] private TMP_Text text_StatHp;          //스탯창의 체력 텍스트
    [SerializeField] private TMP_Text text_StatAtk;         //스탯창의 공격력 텍스트
    [SerializeField] private TMP_Text text_StatAtkRange;    //스탯창의 사거리 텍스트
    [SerializeField] private TMP_Text text_StatAtkDelay;    //스탯창의 공격속도 텍스트

    #region hpBar 관련 메서드
    //hp를 갱신하여 Bar로 보여주는 메서드
    public void SetHpBar(float currentHp, float maxHp)
    {
        if(hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }

        hpCoroutine = StartCoroutine(FadeEffectHpBar(currentHp, maxHp));
        textHp.text = $"{currentHp} / {maxHp}";
    }

    //체력바가 내려가는 모션 이펙트
    private IEnumerator FadeEffectHpBar(float currentHp, float maxHp)
    {
        float timer = 0f;
        Debug.Log(currentHp);
        Debug.Log(maxHp);

        float beforeAmount = imageHp.fillAmount;
        float afterAmount = currentHp / maxHp;

        while(timer < fadeTime)
        {
            timer += Time.deltaTime;
            imageHp.fillAmount = Mathf.Lerp(beforeAmount, afterAmount, timer / fadeTime);
            yield return null;
        }
    }
    #endregion

    #region 스탯창 관련 메서드
    public void SetStatUI(BaseStat stat)
    {
        if (text_StatHp != null)
            text_StatHp.text = $"HP : {stat.GetStat(StatType.HP)}";

        if (text_StatAtk != null)
            text_StatAtk.text = $"Atk : {stat.GetStat(StatType.AttackDamage)}";

        if (text_StatAtkRange != null)
            text_StatAtkRange.text = $"AtkRange : {stat.GetStat(StatType.AttackRange)}";

        if (text_StatAtkDelay != null)
            text_StatAtkDelay.text = $"AtkDelay : {stat.GetStat(StatType.AttackDelay)}";
    }
    #endregion

    private void OnDisable()
    {
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }
    }
}
