using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Upgrade : MonoBehaviour
{
    //체력 업그레이드 관련
    [SerializeField] private TMP_Text text_HpType;
    [SerializeField] private TMP_Text text_HpValue;
    [SerializeField] private TMP_Text text_HpPrice;
    [SerializeField] private Button button_HP;

    //스피드 업그레이드 관련
    [SerializeField] private TMP_Text text_SpeedType;
    [SerializeField] private TMP_Text text_SpeedValue;
    [SerializeField] private TMP_Text text_SpeedPrice;
    [SerializeField] private Button button_Speed;

    //시야각 업그레이드 관련
    [SerializeField] private TMP_Text text_ViewAngleType;
    [SerializeField] private TMP_Text text_ViewAngleValue;
    [SerializeField] private TMP_Text text_ViewAnglePrice;
    [SerializeField] private Button button_ViewAngle;

    //소지 골드 텍스트
    [SerializeField] private TMP_Text text_playerGold;

    //체력 업그레이드 버튼 클릭
    public void OnClickHPUpgrade()
    {
        if (GameManager.Instance.HpUpgradeData.datas[GameManager.Instance.HpLevel + 1].price > GameManager.Instance.Gold)
        {
            return;
        }

        GameManager.Instance.HPUpgrade();
        SetUpgradeSlot(GameManager.Instance.HpUpgradeData, button_HP, text_HpType, text_HpValue, text_HpPrice, GameManager.Instance.HpLevel);
    }

    //이동 속도 업그레이드 버튼 클릭
    public void OnClickSpeedUpgrade()
    {
        if (GameManager.Instance.SpeedUpgradeData.datas[GameManager.Instance.SpeedLevel + 1].price > GameManager.Instance.Gold)
        {
            return;
        }

        GameManager.Instance.SpeedUpgrade();
        SetUpgradeSlot(GameManager.Instance.SpeedUpgradeData, button_Speed, text_SpeedType, text_SpeedValue, text_SpeedPrice, GameManager.Instance.SpeedLevel);
    }

    //시야각 업그레이드 버튼 클릭
    public void OnClickViewAngleUpgrade()
    {
        if (GameManager.Instance.ViewAngleUpgradeData.datas[GameManager.Instance.ViewAngleLevel + 1].price > GameManager.Instance.Gold)
        {
            return;
        }

        GameManager.Instance.ViewAngleUpgrade();
        SetUpgradeSlot(GameManager.Instance.ViewAngleUpgradeData, button_ViewAngle, text_ViewAngleType, text_ViewAngleValue, text_ViewAnglePrice, GameManager.Instance.ViewAngleLevel);
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 업그레이드 UI 슬롯을 설정하는 메서드
    /// </summary>
    /// <param name="data"> 스탯 레벨 테이블 데이터 </param>
    /// <param name="type"> 어떤 스탯인지 적을 TMP_Text </param>
    /// <param name="value"> 변경 될 수치를 적을 TMP_Text </param>
    /// <param name="price"> 업그레이드에 필요한 비용을 적을 TMP_Text </param>
    /// <param name="level"> 해당 스탯의 현재 레벨 </param>
    private void SetUpgradeSlot(SOStatData data, Button button, TMP_Text type, TMP_Text value, TMP_Text price, int level)
    {
        type.text = data.upgradeName;

        if(data.datas.Count - 1 <= level)
        {
            value.text = $"<color=green>{data.datas[level].amount}</color>";
            price.text = "최대레벨";
            button.interactable = false;
        }
        else
        {
            value.text = $"{data.datas[level].amount} → <color=green>{data.datas[level + 1].amount}</color>";
            price.text = data.datas[level + 1].price.ToString();
            button.interactable = true;
        }

        text_playerGold.text = GameManager.Instance.Gold.ToString();
    }

    private void OnEnable()
    {
        //각 업그레이드 슬롯 갱신
        SetUpgradeSlot(GameManager.Instance.HpUpgradeData, button_HP, text_HpType, text_HpValue, text_HpPrice, GameManager.Instance.HpLevel);
        SetUpgradeSlot(GameManager.Instance.SpeedUpgradeData, button_Speed, text_SpeedType, text_SpeedValue, text_SpeedPrice, GameManager.Instance.SpeedLevel);
        SetUpgradeSlot(GameManager.Instance.ViewAngleUpgradeData, button_ViewAngle, text_ViewAngleType, text_ViewAngleValue, text_ViewAnglePrice, GameManager.Instance.ViewAngleLevel);
    }
}
