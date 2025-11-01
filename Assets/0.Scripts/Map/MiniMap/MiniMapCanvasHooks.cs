using UnityEngine;
using TMPro;

//�̴ϸ� UI ��Ҹ� �����ϴ� �� ������Ʈ
public class MiniMapCanvasHooks : MonoBehaviour
{
    //�̴ϸ� �г� ������Ʈ
    public GameObject _miniMapPanel;

    //��� RectTransform
    public RectTransform _miniMapBackground;

    //��Ŀ���� ��ġ�� �θ� Ʈ������
    public Transform _miniMapMarkersParent;

    //�÷��̾� ������ RectTransform
    public RectTransform _miniMapPlayerIcon;

    //�޽��� �ؽ�Ʈ (TMP)
    public TMP_Text _miniMapMessageText;


    //Awake �� �ڵ����� ������ ����
    [SerializeField] private bool _miniMapHideOnAwake = true;

    //Awake �� ���� �ڵ� ���� �� �ʱ� ��Ȱ��ȭ ó��
    private void Awake()
    {
        //Panel ã��
        if (_miniMapPanel == null)
        {
            Transform t = transform.Find("Panel");

            if (t != null)
            {
                _miniMapPanel = t.gameObject;
            }
        }

        //Background ã��
        if (_miniMapBackground == null)
        {
            _miniMapBackground = transform.Find("Background") as RectTransform;
        }

        //Markers �θ� ã��
        if (_miniMapMarkersParent == null)
        {
            _miniMapMarkersParent = transform.Find("Markers");
        }

        //PlayerIcon ã��
        if (_miniMapPlayerIcon == null)
        {
            _miniMapPlayerIcon = transform.Find("PlayerIcon") as RectTransform;
        }

        //MessageText ã��
        if (_miniMapMessageText == null)
        {
            Transform t = transform.Find("MessageText");

            if (t != null)
            {
                _miniMapMessageText = t.GetComponent<TMP_Text>();
            }
        }

        //hideOnAwake�� true�̸� �ʱ� ��Ȱ��ȭ
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

    //�г� Ȱ��/��Ȱ�� ���
    public void SetActive(bool active)
    {
        if (_miniMapPanel != null)
        {
            _miniMapPanel.SetActive(active);
        }
    }
}
