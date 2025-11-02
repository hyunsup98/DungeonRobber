using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI 오브젝트에 이 스크립트를 넣으면 지정한 AudioClip을 재생하고 Text의 Bold를 껐다 키는 효과를 줄 수 있습니다.
/// </summary>
public class UI_MouseOnEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private TMP_Text text;
    [SerializeField] private bool isTextEffect;     //마우스를 올렸을 때 텍스트의 크기가 커지는 효과를 넣을 것인지

    //현재 UI 오브젝트에 마우스 포인터를 올렸을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(clip != null)
        {
            SoundManager.Instance.PlaySoundEffect(clip);
        }

        if(isTextEffect)
        {
            if(text != null)
            {
                text.fontStyle = FontStyles.Bold;
            }
        }
    }

    //현재 UI 오브젝트에서 마우스 포인터를 내렸을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if(isTextEffect)
        {
            if(text != null)
            {
                text.fontStyle = FontStyles.Normal;
            }
        }
    }
}
