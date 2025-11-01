using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider soundEffectSlider;

    //����� ���� ����
    public void OnBGMVolumeChanged()
    {
        SoundManager.Instance.SetBGMVolume(bgmSlider.value);
    }

    //ȿ���� ���� ����
    public void OnSoundEffectVolumeChanged()
    {
        SoundManager.Instance.SetSoundEffectVolume(soundEffectSlider.value);
    }

    //���� ��ư Ŭ��
    public void OnClickAccept()
    {
        UIManager.Instance.OnOffUI(gameObject, false);
    }
}
