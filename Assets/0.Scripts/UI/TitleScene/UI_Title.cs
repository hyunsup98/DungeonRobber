using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ÿ��Ʋ ������ �����ϴ� UI�� �����ϴ� Ŭ����
/// </summary>
public class UI_Title : MonoBehaviour
{
    [SerializeField] private string gameSceneName;

    //���� ���� ��ư�� Ŭ������ �� ȣ��
    public void OnClickGameStart()
    {
        if (string.IsNullOrEmpty(gameSceneName)) return;

        SceneManager.LoadScene(gameSceneName);
    }

    //���� ��ư�� Ŭ������ �� ȣ��
    public void OnClickSettings()
    {

    }

    //���� ������ ��ư�� Ŭ������ �� ȣ��
    public void OnClickExit()
    {
        Application.Quit();
    }
}
