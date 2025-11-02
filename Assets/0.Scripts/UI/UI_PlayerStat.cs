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
    [SerializeField] private Image imageHp;                     //hpBar 이미지
    [SerializeField] private TMP_Text textHp;                   //hpBar 텍스트 - "현재체력 / 최대체력"
    [SerializeField] private float hpLerpTime = 0.3f;           //체력이 깎일 때 깎이는 모션이 걸리는 시간
    private Coroutine hpCoroutine;

    [Header("stamina 관련 변수들")]
    [SerializeField] private Image imageStamina;                //스태미너바 이미지
    [SerializeField] private float staminaLerpTime = 0.15f;     //스태미너 러프 시간
    private Coroutine staminaCoroutine;

    #region hpBar 관련 메서드
    //hp를 갱신하여 Bar로 보여주는 메서드
    public void SetHpBar(float currentHp, float maxHp)
    {
        if(hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }

        hpCoroutine = StartCoroutine(FadeEffectBar(imageHp, currentHp, maxHp, hpLerpTime));
        textHp.text = $"{currentHp} / {maxHp}";
    }

    public void SetStaminaBar(float stamina, float maxStamina)
    {
        if(staminaCoroutine != null)
        {
            StopCoroutine(staminaCoroutine);
        }

        staminaCoroutine = StartCoroutine(FadeEffectBar(imageStamina, stamina, maxStamina, staminaLerpTime));
    }

    //이미지 바가 움직이는 모션 이펙트
    private IEnumerator FadeEffectBar(Image bar, float currentValue, float maxValue, float time)
    {
        float timer = 0f;

        float beforeAmount = bar.fillAmount;
        float afterAmount = currentValue / maxValue;

        while(timer < time)
        {
            timer += Time.deltaTime;
            bar.fillAmount = Mathf.Lerp(beforeAmount, afterAmount, timer / time);
            yield return null;
        }
    }
    #endregion

    private void OnDisable()
    {
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }

        if(staminaCoroutine != null)
        {
            StopCoroutine(staminaCoroutine);
        }
    }
}
