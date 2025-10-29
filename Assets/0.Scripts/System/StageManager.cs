using UnityEngine;

/// <summary>
/// �� ������ �̵����� �� ó���� ������ �ʱ� ������ �����ϴ� Ŭ����
/// ex) ���� �˸��� ��� ��� ���
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;

    private void Start()
    {
        PlaySceneBGM();
    }

    //������ �̵����� �� ����� BGM Ʋ��
    private void PlaySceneBGM()
    {
        if (bgmClip == null) return;

        SoundManager.Instance.PlayBGM(bgmClip, 0.5f);
    }
}
