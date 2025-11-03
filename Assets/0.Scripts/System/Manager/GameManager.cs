using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Title,          //타이틀 씬 상태
    Base,           //베이스 씬 상태
    Dungeon,        //던전(인게임) 상태
}

public class GameManager : Singleton<GameManager>
{
    #region 게임 상태 관리 델리게이트 및 프로퍼티
    public event Action onSceneChanged;         //어떤 씬이든 변경되었을 때 실행할 액션 이벤트
    public event Action onGameStateTitle;       //GameState가 Title이 되었을 때 실행할 액션 이벤트
    public event Action onGameStateBase;        //GameState가 Base가 되었을 때 실행할 액션 이벤트
    public event Action onGameStateDungeon;     //GameState가 Dungeon이 되었을 때 실행할 액션 이벤트

    private GameState currentGameState;
    public GameState CurrentGameState
    {
        get { return currentGameState; }
        set
        {
            if (value == currentGameState) return;

            if (value == GameState.Title)
            {
                SceneManager.LoadScene("TitleScene");
                onGameStateTitle?.Invoke();
            }
            else if (value == GameState.Base)
            {
                SceneManager.LoadScene("BaseScene");
                onGameStateBase?.Invoke();
            }
            else if (value == GameState.Dungeon)
            {
                SceneManager.LoadScene("DungeonScene");
                onGameStateDungeon?.Invoke();
            }

            onSceneChanged?.Invoke();
            currentGameState = value;
        }
    }
    #endregion

    #region 플레이어 스탯 레벨 업 데이터
    [field: SerializeField] public SOStatData HpUpgradeData { get; private set; }
    public int HpLevel { get; private set; } = 0;
    [field: SerializeField] public SOStatData SpeedUpgradeData { get; private set; }
    public int SpeedLevel { get; private set; } = 0;
    [field: SerializeField] public SOStatData ViewAngleUpgradeData { get; private set; }
    public int ViewAngleLevel { get; private set; } = 0;
    #endregion

    public event Action variationGoldAction;        //골드가 변했을 때 할 이벤트 액션
    private int gold = 0;       //플레이어 소지 골드
    public int Gold
    {
        get { return gold; }
        set
        {
            if(value < 0)
            {
                value = 0;
            }

            gold = value;
            variationGoldAction?.Invoke();
        }
    }

    private void Awake()
    {
        SingletonInit();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Gold += 500000;
        }
    }

    #region 플레이어 스탯 업그레이드 관련 메서드
    public void HPUpgrade()
    {
        Gold -= HpUpgradeData.datas[HpLevel + 1].price;
        HpLevel = Mathf.Clamp(++HpLevel, 0, HpUpgradeData.datas.Count - 1);
        Player_Controller.Instance.SetPlayerStat(StatType.HP, HpUpgradeData.datas[HpLevel].amount);
    }

    public void SpeedUpgrade()
    {
        Gold -= SpeedUpgradeData.datas[SpeedLevel + 1].price;
        SpeedLevel = Mathf.Clamp(++SpeedLevel, 0, SpeedUpgradeData.datas.Count - 1);
        Player_Controller.Instance.SetPlayerStat(StatType.MoveSpeed, SpeedUpgradeData.datas[SpeedLevel].amount);
    }

    public void ViewAngleUpgrade()
    {
        Gold -= ViewAngleUpgradeData.datas[ViewAngleLevel + 1].price;
        ViewAngleLevel = Mathf.Clamp(++ViewAngleLevel, 0, ViewAngleUpgradeData.datas.Count - 1);
        Player_Controller.Instance.fieldOfView.SetViewAngle(ViewAngleUpgradeData.datas[ViewAngleLevel].amount);
    }
    #endregion
}
