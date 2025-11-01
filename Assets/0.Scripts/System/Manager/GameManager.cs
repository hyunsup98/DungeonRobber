using System;
using UnityEngine;

public enum GameState
{
    Title,          //Ÿ��Ʋ �� ����
    Base,           //���̽� �� ����
    Dungeon,        //����(�ΰ���) ����
    Pause           //�Ͻ����� ����
}

public class GameManager : Singleton<GameManager>
{
    #region ���� ���� ���� ��������Ʈ �� ������Ƽ
    public event Action onGameStateTitle;       //GameState�� Title�� �Ǿ��� �� ������ �׼�
    public event Action onGameStateBase;        //GameState�� Base�� �Ǿ��� �� ������ �׼�
    public event Action onGameStateDungeon;     //GameState�� Dungeon�� �Ǿ��� �� ������ �׼�
    public event Action onGameStatePause;       //GameState�� Pause�� �Ǿ��� �� ������ �׼�

    private GameState currentGameState;
    public GameState CurrentGameState
    {
        get { return currentGameState; }
        set
        {
            if (value == currentGameState) return;

            if(value == GameState.Title)
            {
                onGameStateTitle?.Invoke();
            }
            else if(value == GameState.Base)
            {
                onGameStateBase?.Invoke();
            }
            else if(value == GameState.Dungeon)
            {
                onGameStateDungeon?.Invoke();
            }
            else if(value == GameState.Pause)
            {
                onGameStatePause?.Invoke();
            }
        }
    }
    #endregion

    private void Awake()
    {
        SingletonInit();
    }
}
