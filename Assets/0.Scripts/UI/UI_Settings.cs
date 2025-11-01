using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider soundEffectSlider;

    //배경음 볼륨 조절
    public void OnBGMVolumeChanged()
    {
        SoundManager.Instance.SetBGMVolume(bgmSlider.value);
    }

    //효과음 볼륨 조절
    public void OnSoundEffectVolumeChanged()
    {
        SoundManager.Instance.SetSoundEffectVolume(soundEffectSlider.value);
    }

    //적용 버튼 클릭
    public void OnClickAccept()
    {
        UIManager.Instance.OnOffUI(gameObject, false);
    }
}
