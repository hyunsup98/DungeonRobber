using UnityEngine;

public class UI_DeadMenu : MonoBehaviour
{
    //부활하기 버튼 클릭
    public void OnClickRevive()
    {
        GameManager.Instance.CurrentGameState = GameState.Base;
    }

    //타이틀로 버튼 클릭
    public void OnClickGoTitle()
    {
        GameManager.Instance.CurrentGameState = GameState.Title;
    }
}
