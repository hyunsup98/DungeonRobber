using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;     //�� ���� ���۵� �� ������ BGM Ŭ��

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
