using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    private void Awake()
    {
        SingletonInit();
        SceneManager.sceneLoaded += OnSceneLoaded;
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
