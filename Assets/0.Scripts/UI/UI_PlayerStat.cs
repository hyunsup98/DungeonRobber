using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾��� hp, ���� ���� UI�� �����ִ� Ŭ����
/// </summary>
public class UI_PlayerStat : MonoBehaviour
{
    [Header("hpBar ���� ������")]
    //hpBar ���� ����
    [SerializeField] private Image imageHp;                 //hpBar �̹���
    [SerializeField] private TMP_Text textHp;               //hpBar �ؽ�Ʈ - "����ü�� / �ִ�ü��"
    [SerializeField] private float fadeTime = 0.3f;         //ü���� ���� �� ���̴� ����� �ɸ��� �ð�
    private Coroutine hpCoroutine;

    [Header("���� ���� ������")]
    [SerializeField] private TMP_Text text_StatHp;          //����â�� ü�� �ؽ�Ʈ
    [SerializeField] private TMP_Text text_StatAtk;         //����â�� ���ݷ� �ؽ�Ʈ
    [SerializeField] private TMP_Text text_StatAtkRange;    //����â�� ��Ÿ� �ؽ�Ʈ
    [SerializeField] private TMP_Text text_StatAtkDelay;    //����â�� ���ݼӵ� �ؽ�Ʈ

    #region hpBar ���� �޼���
    //hp�� �����Ͽ� Bar�� �����ִ� �޼���
    public void SetHpBar(float currentHp, float maxHp)
    {
        if(hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }

        hpCoroutine = StartCoroutine(FadeEffectHpBar(currentHp, maxHp));
        textHp.text = $"{currentHp} / {maxHp}";
    }

    //ü�¹ٰ� �������� ��� ����Ʈ
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

    #region ����â ���� �޼���
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
