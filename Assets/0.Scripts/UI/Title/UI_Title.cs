using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 타이틀 씬에서 제어할 UI 클래스
/// </summary>
public class UI_Title : MonoBehaviour
{
    [Header("다음 씬 이름")]
    [SerializeField] private string nextSceneName;      //다음 씬 이름 → 게임 스타트 버튼을 눌렀을 때 이동할 씬 이름

    //게임 스타트 버튼을 클릭했을 때
    public void OnClickGameStart()
    {
        if (string.IsNullOrEmpty(nextSceneName)) return;

        SceneManager.LoadScene(nextSceneName);
    }

    //설정 버튼을 클릭했을 때
    public void OnClickSettings()
    {
        Debug.Log("설정창 열림");
    }

    //게임 나가기 버튼을 클릭했을 때
    public void OnClickExit()
    {
        Application.Quit();
    }
}
