using UnityEngine;
using TMPro;

//미니맵 UI 요소를 참조하는 훅 컴포넌트
public class MiniMapCanvasHooks : MonoBehaviour
{
    //미니맵 패널 오브젝트
    public GameObject panel;

    //배경 RectTransform
    public RectTransform background;

    //마커들이 위치할 부모 트랜스폼
    public Transform markersParent;

    //플레이어 아이콘 RectTransform
    public RectTransform playerIcon;

    //메시지 텍스트 (TMP)
    public TMP_Text messageText;

    //Awake 시 자동으로 숨길지 여부
    [SerializeField] private bool hideOnAwake = true;

    //Awake 시 참조 자동 연결 및 초기 비활성화 처리
    private void Awake()
    {
        //Panel 찾기
        if (panel == null)
        {
            Transform t = transform.Find("Panel");

            if (t != null)
            {
                panel = t.gameObject;
            }
        }

        //Background 찾기
        if (background == null)
        {
            background = transform.Find("Background") as RectTransform;
        }

        //Markers 부모 찾기
        if (markersParent == null)
        {
            markersParent = transform.Find("Markers");
        }

        //PlayerIcon 찾기
        if (playerIcon == null)
        {
            playerIcon = transform.Find("PlayerIcon") as RectTransform;
        }

        //MessageText 찾기
        if (messageText == null)
        {
            Transform t = transform.Find("MessageText");

            if (t != null)
            {
                messageText = t.GetComponent<TMP_Text>();
            }
        }

        //hideOnAwake가 true이면 초기 비활성화
        if (hideOnAwake == true)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }

            if (playerIcon != null)
            {
                playerIcon.gameObject.SetActive(false);
            }

            if (messageText != null)
            {
                messageText.gameObject.SetActive(false);
            }
        }
    }

    //패널 활성/비활성 토글
    public void SetActive(bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}
