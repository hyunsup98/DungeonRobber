using UnityEngine;
using TMPro;

//미니맵 UI 요소를 참조하는 훅 컴포넌트
public class MiniMapCanvasHooks : MonoBehaviour
{
    //미니맵 패널 오브젝트
    public GameObject _miniMapPanel;

    //배경 RectTransform
    public RectTransform _miniMapBackground;

    //마커들이 위치할 부모 트랜스폼
    public Transform _miniMapMarkersParent;

    //플레이어 아이콘 RectTransform
    public RectTransform _miniMapPlayerIcon;

    //메시지 텍스트 (TMP)
    public TMP_Text _miniMapMessageText;


    //Awake 시 자동으로 숨길지 여부
    [SerializeField] private bool _miniMapHideOnAwake = true;

    //Awake 시 참조 자동 연결 및 초기 비활성화 처리
    private void Awake()
    {
        //Panel 찾기
        if (_miniMapPanel == null)
        {
            Transform t = transform.Find("Panel");

            if (t != null)
            {
                _miniMapPanel = t.gameObject;
            }
        }

        //Background 찾기
        if (_miniMapBackground == null)
        {
            _miniMapBackground = transform.Find("Background") as RectTransform;
        }

        //Markers 부모 찾기
        if (_miniMapMarkersParent == null)
        {
            _miniMapMarkersParent = transform.Find("Markers");
        }

        //PlayerIcon 찾기
        if (_miniMapPlayerIcon == null)
        {
            _miniMapPlayerIcon = transform.Find("PlayerIcon") as RectTransform;
        }

        //MessageText 찾기
        if (_miniMapMessageText == null)
        {
            Transform t = transform.Find("MessageText");

            if (t != null)
            {
                _miniMapMessageText = t.GetComponent<TMP_Text>();
            }
        }

        //hideOnAwake가 true이면 초기 비활성화
        if (_miniMapHideOnAwake == true)
        {
            if (_miniMapPanel != null)
            {
                _miniMapPanel.SetActive(false);
            }

            if (_miniMapPlayerIcon != null)
            {
                _miniMapPlayerIcon.gameObject.SetActive(false);
            }

            if (_miniMapMessageText != null)
            {
                _miniMapMessageText.gameObject.SetActive(false);
            }
        }
    }

    //패널 활성/비활성 토글
    public void SetActive(bool active)
    {
        if (_miniMapPanel != null)
        {
            _miniMapPanel.SetActive(active);
        }
    }
}
