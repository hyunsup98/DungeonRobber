using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 위험 텍스트를 알려주는 스크립트 
/// </summary>

public class DangerouText : MonoBehaviour, IDangerousTimeObserver
{
    [SerializeField] TextMeshProUGUI dangerousTMP;

    [Tooltip("위험 시간 알림으로 나올 텍스트 ")]
    [SerializeField] string inputText;
    [SerializeField] TimerFunc timerFunc; // 타이머 기능 
    

    Coroutine dangerCouroutine;
    WaitForSeconds textDelay = new WaitForSeconds(5f); //위험 텍스트 얼마나 띄울지 

    private void Awake()
    {
        timerFunc.addDangerousTimeEvent(this);

    }

    private void OnDestroy()
    {
        timerFunc.delDangerousTimeEvent(this);
    }
    public void OnDangerousTimeReached()
    {
        if(dangerCouroutine == null)
        {
            dangerCouroutine = StartCoroutine(nameof(dangerTextController));
        }
    }

    public void ClearText() //텍스트 삭제 
    {
        dangerousTMP.text = "";
    }

    IEnumerator dangerTextController()
    {
        dangerousTMP.color = Color.red;
        dangerousTMP.text = inputText;

        yield return textDelay;
        
        ClearText();

    }
}
