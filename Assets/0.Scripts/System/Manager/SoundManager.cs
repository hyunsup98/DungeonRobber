using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    private void Awake()
    {
        SingletonInit();
    }

    public void PlayBGM(AudioClip clip, float volume)
    {
        if (bgmAudioSource == null || clip == null) return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.volume = volume;
        bgmAudioSource.Play();
    }

    public void PlayerSoundEffect(AudioClip clip, float volume)
    {
        if (soundEffectAudioSource == null || clip == null) return;

        soundEffectAudioSource.volume = volume;
        soundEffectAudioSource.PlayOneShot(clip);
    }
}
