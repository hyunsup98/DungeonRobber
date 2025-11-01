using UnityEngine;
using TMPro;

//�̴ϸ� UI ��Ҹ� �����ϴ� �� ������Ʈ
public class MiniMapCanvasHooks : MonoBehaviour
{
    //�̴ϸ� �г� ������Ʈ
    public GameObject panel;

    //��� RectTransform
    public RectTransform background;

    //��Ŀ���� ��ġ�� �θ� Ʈ������
    public Transform markersParent;

    //�÷��̾� ������ RectTransform
    public RectTransform playerIcon;

    //�޽��� �ؽ�Ʈ (TMP)
    public TMP_Text messageText;

    //Awake �� �ڵ����� ������ ����
    [SerializeField] private bool hideOnAwake = true;

    //Awake �� ���� �ڵ� ���� �� �ʱ� ��Ȱ��ȭ ó��
    private void Awake()
    {
        //Panel ã��
        if (panel == null)
        {
            Transform t = transform.Find("Panel");

            if (t != null)
            {
                panel = t.gameObject;
            }
        }

        //Background ã��
        if (background == null)
        {
            background = transform.Find("Background") as RectTransform;
        }

        //Markers �θ� ã��
        if (markersParent == null)
        {
            markersParent = transform.Find("Markers");
        }

        //PlayerIcon ã��
        if (playerIcon == null)
        {
            playerIcon = transform.Find("PlayerIcon") as RectTransform;
        }

        //MessageText ã��
        if (messageText == null)
        {
            Transform t = transform.Find("MessageText");

            if (t != null)
            {
                messageText = t.GetComponent<TMP_Text>();
            }
        }

        //hideOnAwake�� true�̸� �ʱ� ��Ȱ��ȭ
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

    //�г� Ȱ��/��Ȱ�� ���
    public void SetActive(bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}
