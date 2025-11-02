using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;     //각 씬이 시작될 때 실행할 BGM 클립
    [SerializeField] private Transform spawnPos;    //플레이어가 처음에 시작할 위치

    private void Start()
    {
        PlayBGM();
        if(Player_Controller.Instance != null && spawnPos != null)
        {
            Player_Controller.Instance.transform.position = spawnPos.position;
        }
    }

    private void PlayBGM()
    {
        if (bgmClip == null) return;

        SoundManager.Instance.PlayBGM(bgmClip);
    }
}
