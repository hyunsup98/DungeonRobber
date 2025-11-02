using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Title,          //타이틀 씬 상태
    Base,           //베이스 씬 상태
    Dungeon,        //던전(인게임) 상태
    Pause           //일시정지 상태
}

public class GameManager : Singleton<GameManager>
{
    #region 게임 상태 관리 델리게이트 및 프로퍼티
    public event Action onSceneChanged;         //어떤 씬이든 변경되었을 때 실행할 액션 이벤트
    public event Action onGameStateTitle;       //GameState가 Title이 되었을 때 실행할 액션 이벤트
    public event Action onGameStateBase;        //GameState가 Base가 되었을 때 실행할 액션 이벤트
    public event Action onGameStateDungeon;     //GameState가 Dungeon이 되었을 때 실행할 액션 이벤트
    public event Action onGameStatePause;       //GameState가 Pause가 되었을 때 실행할 액션 이벤트

    private GameState currentGameState;
    public GameState CurrentGameState
    {
        get { return currentGameState; }
        set
        {
            if (value == currentGameState) return;

            if(value == GameState.Title)
            {
                SceneManager.LoadScene("TitleScene");
                onGameStateTitle?.Invoke();
            }
            else if(value == GameState.Base)
            {
                SceneManager.LoadScene("BaseScene");
                onGameStateBase?.Invoke();
            }
            else if(value == GameState.Dungeon)
            {
                SceneManager.LoadScene("DungeonScene");
                onGameStateDungeon?.Invoke();
            }
            else if(value == GameState.Pause)
            {
                onGameStatePause?.Invoke();
            }

            currentGameState = value;
        }
    }
    #endregion

    private void Awake()
    {
        SingletonInit();
    }

}
