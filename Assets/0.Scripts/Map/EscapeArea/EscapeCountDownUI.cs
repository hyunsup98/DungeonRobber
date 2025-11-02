using TMPro;
using UnityEngine;

public class EscapeCountDownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _escapeUIPanel;
    [SerializeField] private TMP_Text _escapeUIMessageText;
    [SerializeField] private TMP_Text _escapeUICountTimerText;


    private void Awake()
    {
        //패널을 지정 안 했으면 자기 자신을 패널로 사용해도 됨
        if (_escapeUIPanel == null) _escapeUIPanel = gameObject;

        _escapeUIPanel.SetActive(false);
    }


    //안내 문구와 함께 카운트다운 UI 표시
    public void EscapeCountDownShow(string message, float durationSeconds)
    {
        if (_escapeUIPanel != null) _escapeUIPanel.SetActive(true);

        if (_escapeUIMessageText != null)
            _escapeUIMessageText.text = message;

        EscapeCountDownSetTime(durationSeconds);
    }


    //남은 시간(초)을 숫자로 갱신
    public void EscapeCountDownSetTime(float secondsLeft)
    {
        if (_escapeUICountTimerText == null) return;

        int second = Mathf.CeilToInt(secondsLeft);
        _escapeUICountTimerText.text = second.ToString();
    }


    //카운트다운 UI 숨김
    public void EscapeCountDownHide()
    {
        if (_escapeUIPanel != null)
            _escapeUIPanel.SetActive(false);
    }
}
