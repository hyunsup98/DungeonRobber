using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 타이틀 씬에서 존재하는 UI를 관리하는 클래스
/// </summary>
public class UI_Title : MonoBehaviour
{
    //게임 시작 버튼을 클릭했을 때 호출
    public void OnClickGameStart()
    {
        GameManager.Instance.CurrentGameState = GameState.Base;
    }

    //세팅 버튼을 클릭했을 때 호출
    public void OnClickSettings()
    {
        UIManager.Instance.settings.gameObject.SetActive(true);
    }

    //게임 나가기 버튼을 클릭했을 때 호출
    public void OnClickExit()
    {
        Application.Quit();
    }
}
