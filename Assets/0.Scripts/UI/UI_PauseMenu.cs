using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PauseMenu : MonoBehaviour
{
    //���� �簳 ��ư Ŭ��
    public void OnClickResume()
    {

    }

    //���� ��ư Ŭ��
    public void OnClickSetting()
    {
        UIManager.Instance.OnOffUI(UIManager.Instance.settings.gameObject, true);
    }

    //Ÿ��Ʋ ���� ��ư Ŭ��
    public void OnClickTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
