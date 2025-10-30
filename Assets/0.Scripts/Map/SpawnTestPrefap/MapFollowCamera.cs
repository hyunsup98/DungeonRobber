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
        //ū �̴ϸ��� ���� �� ���д�
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
            // ī�޶� Y ����, �÷��̾� ��ġ ���󰡱�
            Vector3 newPos = _miniMapTagetPlayer.position;
            newPos.y = transform.position.y;
            transform.position = newPos;

            // �̴ϸ� ������ ȸ��
            if (_miniMapPlayerIcon != null)
            {
                _miniMapPlayerIcon.rotation = Quaternion.Euler(90f, _miniMapTagetPlayer.eulerAngles.y, 0f);
            }
        }

        // M �Ǵ� Tab Ű�� ���� �̴ϸ� ��ȯ
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMiniMap();
        }
    }

    //�̴ϸ� Ȯ��/��� ��ȯ �޼���
    void ToggleMiniMap()
    {
        _isLargeMap = !_isLargeMap;  // �� �̰� �ٽ�! ���� ����

        if (_isLargeMap)
        {
            // ū �̴ϸ� �ѱ�
            if (_largeMapUI != null)
                _largeMapUI.SetActive(true);

            // ���� �̴ϸ� ����
            if (_smallMapUI != null)
                _smallMapUI.SetActive(false);

            // ū ���� �� ī�޶� �� �ƿ�
            if (_minimapCamera != null && _minimapCamera.orthographic)
                _minimapCamera.orthographicSize = 100f;
        }
        else
        {
            // ū �̴ϸ� ����
            if (_largeMapUI != null)
                _largeMapUI.SetActive(false);

            // ���� �̴ϸ� �ѱ�
            if (_smallMapUI != null)
                _smallMapUI.SetActive(true);

            // ���� ���� �� ī�޶� �� ��
            if (_minimapCamera != null && _minimapCamera.orthographic)
                _minimapCamera.orthographicSize = 30f;
        }
    }
}
