using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    private void Awake()
    {
        SingletonInit();
    }

    private void Start()
    {
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

    //배경음과 효과음이 재생중이라면 중지
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAudioSource(bgmAudioSource);
        StopAudioSource(soundEffectAudioSource);
    }

    /// <summary>
    /// 오디오 소스가 플레이 중이면 중지하는 메서드
    /// 씬을 이동하는 등의 오디오를 중지해야하는 경우 사용
    /// </summary>
    /// <param name="source"></param>
    private void StopAudioSource(AudioSource source)
    {
        if(source != null)
        {
            if(source.isPlaying)
            {
                source.Stop();
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
