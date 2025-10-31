using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;     //각 씬이 시작될 때 실행할 BGM 클립

    private void Start()
    {
        PlayBGM();
    }

    private void PlayBGM()
    {
        if (bgmClip == null) return;

        SoundManager.Instance.PlayBGM(bgmClip, 0.3f);
    }
}
