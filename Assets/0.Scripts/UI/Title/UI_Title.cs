using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ÿ��Ʋ ������ ������ UI Ŭ����
/// </summary>
public class UI_Title : MonoBehaviour
{
    [Header("���� �� �̸�")]
    [SerializeField] private string nextSceneName;      //���� �� �̸� �� ���� ��ŸƮ ��ư�� ������ �� �̵��� �� �̸�

    //���� ��ŸƮ ��ư�� Ŭ������ ��
    public void OnClickGameStart()
    {
        if (string.IsNullOrEmpty(nextSceneName)) return;

        SceneManager.LoadScene(nextSceneName);
    }

    //���� ��ư�� Ŭ������ ��
    public void OnClickSettings()
    {
        Debug.Log("����â ����");
    }

    //���� ������ ��ư�� Ŭ������ ��
    public void OnClickExit()
    {
        Application.Quit();
    }
}
