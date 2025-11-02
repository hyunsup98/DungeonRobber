using UnityEngine;

public class Presenter_Settings
{
    private SoundManager soundManager;
    private UI_Settings settings;

    public Presenter_Settings(SoundManager soundManager, UI_Settings settings)
    {
        this.soundManager = soundManager;
        this.settings = settings;

        this.soundManager.onChangedVolume += ChangeVolume;
    }

    private void ChangeVolume(SoundType type, float value)
    {
        settings.SetSliderValue(type, value);
    }
}
