using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider soundEffectSlider;

    //배경음 볼륨 조절
    public void OnBGMVolumeChanged()
    {
        SoundManager.Instance.SetSoundVolume(SoundType.BGM, bgmSlider.value);
    }

    //효과음 볼륨 조절
    public void OnSoundEffectVolumeChanged()
    {
        SoundManager.Instance.SetSoundVolume(SoundType.SoundEffect, soundEffectSlider.value);
    }

    //적용 버튼 클릭
    public void OnClickAccept()
    {
        UIManager.Instance.OnOffUI(gameObject, false);
    }

    //SoundManager의 Audiosource(배경음, 효과음)의 볼륨이 변경되었을 때 볼륨 설정 슬라이더 값도 변경해주는 메서드
    public void SetSliderValue(SoundType type, float value)
    {
        switch(type)
        {
            case SoundType.BGM:
                bgmSlider.value = value;
                break;
            case SoundType.SoundEffect:
                soundEffectSlider.value = value;
                break;
        }
    }
}
