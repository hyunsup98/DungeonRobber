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

    //������� ȿ������ ������̶�� ����
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAudioSource(bgmAudioSource);
        StopAudioSource(soundEffectAudioSource);
    }

    /// <summary>
    /// ����� �ҽ��� �÷��� ���̸� �����ϴ� �޼���
    /// ���� �̵��ϴ� ���� ������� �����ؾ��ϴ� ��� ���
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
