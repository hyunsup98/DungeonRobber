using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SoundType
{
    BGM,            //배경음
    SoundEffect,    //효과음
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    public event Action<SoundType, float> onChangedVolume;

    private void Awake()
    {
        SingletonInit();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmAudioSource == null || clip == null) return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (soundEffectAudioSource == null || clip == null) return;

        soundEffectAudioSource.PlayOneShot(clip);
    }

    public void SetSoundVolume(SoundType type, float volume)
    {
        switch(type)
        {
            case SoundType.BGM:
                bgmAudioSource.volume = volume;
                onChangedVolume?.Invoke(SoundType.BGM, bgmAudioSource.volume);
                break; ;
            case SoundType.SoundEffect:
                soundEffectAudioSource.volume = volume;
                onChangedVolume?.Invoke(SoundType.SoundEffect, soundEffectAudioSource.volume);
                break;
        }
    }

    //씬이 로드될 때 실행할 메서드
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAudioSource(bgmAudioSource);
        StopAudioSource(soundEffectAudioSource);
    }

    private void StopAudioSource(AudioSource source)
    {
        if(source != null &&  source.isPlaying)
        {
            source.Stop();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
