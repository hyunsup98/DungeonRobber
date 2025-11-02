using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PauseMenu : MonoBehaviour
{
    //게임 재개 버튼 클릭
    public void OnClickResume()
    {
        gameObject.SetActive(false);
    }

    //세팅 버튼 클릭
    public void OnClickSetting()
    {
        UIManager.Instance.OnOffUI(UIManager.Instance.settings.gameObject, true);
    }

    //타이틀 가기 버튼 클릭
    public void OnClickTitle()
    {
        gameObject.SetActive(false);
        GameManager.Instance.CurrentGameState = GameState.Title;
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
