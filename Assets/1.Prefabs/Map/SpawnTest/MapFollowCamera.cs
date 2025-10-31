using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform _miniMapTagetPlayer;
    public Transform _miniMapPlayerIcon;

    [SerializeField] GameObject _smallMapUI;
    [SerializeField] GameObject _largeMapUI;
    [SerializeField] public Camera _minimapCamera;

    private bool _isLargeMap = false;



    private void Start()
    {
        //큰 미니맵은 시작 시 꺼둔다
        _minimapCamera = GetComponent<Camera>();
        if (_largeMapUI != null)
        {
            _largeMapUI.SetActive(false);
        }

        if (_smallMapUI != null)
        {
            _smallMapUI.SetActive(true);
        }
    }


    void LateUpdate()
    {
        if (_miniMapTagetPlayer != null)
        {
            // 카메라 Y 고정, 플레이어 위치 따라가기
            Vector3 newPos = _miniMapTagetPlayer.position;
            newPos.y = transform.position.y;
            transform.position = newPos;

            // 미니맵 아이콘 회전
            if (_miniMapPlayerIcon != null)
            {
                _miniMapPlayerIcon.rotation = Quaternion.Euler(90f, _miniMapTagetPlayer.eulerAngles.y, 0f);
            }
        }

        // M 또는 Tab 키를 눌러 미니맵 전환
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMiniMap();
        }
    }

    //미니맵 확대/축소 전환 메서드
    void ToggleMiniMap()
    {
        _isLargeMap = !_isLargeMap;  // ← 이게 핵심! 상태 반전

        if (_isLargeMap)
        {
            // 큰 미니맵 켜기
            if (_largeMapUI != null)
                _largeMapUI.SetActive(true);

            // 작은 미니맵 끄기
            if (_smallMapUI != null)
                _smallMapUI.SetActive(false);

            // 큰 맵일 때 카메라 줌 아웃
            if (_minimapCamera != null && _minimapCamera.orthographic)
                _minimapCamera.orthographicSize = 100f;
        }
        else
        {
            // 큰 미니맵 끄기
            if (_largeMapUI != null)
                _largeMapUI.SetActive(false);

            // 작은 미니맵 켜기
            if (_smallMapUI != null)
                _smallMapUI.SetActive(true);

            // 작은 맵일 때 카메라 줌 인
            if (_minimapCamera != null && _minimapCamera.orthographic)
                _minimapCamera.orthographicSize = 30f;
        }
    }
}
