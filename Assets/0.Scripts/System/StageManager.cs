using UnityEngine;

/// <summary>
/// 각 씬으로 이동했을 때 처음에 실행할 초기 세팅을 관리하는 클래스
/// ex) 씬에 알맞은 브금 재생 등등
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;

    private void Start()
    {
        PlaySceneBGM();
    }

    //씬으로 이동했을 때 재생할 BGM 틀기
    private void PlaySceneBGM()
    {
        if (bgmClip == null) return;

        SoundManager.Instance.PlayBGM(bgmClip, 0.5f);
    }
}
