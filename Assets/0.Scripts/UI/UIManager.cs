using System.Collections;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region MVP ���Ͽ� ����ϴ� ������

    //�÷��̾� hp��, ����â
    [SerializeField] private UI_PlayerStat playerStat;      //ü�¹ٿ� ����â�� ��� �����ϴ� Ŭ����
    [SerializeField] private GameObject playerStatUI;       //����â�� ������ ������Ʈ
    private Presenter_PlayerStat presenter_PlayerStat;

    #endregion

    #region �ܼ��� ����̶� ���� �������� ������

    [field : SerializeField] public UI_InteractiveMessage textInteractive { get; private set; } //��ȣ�ۿ� Ű �ؽ�Ʈ Ŭ����
    [field : SerializeField] public UI_Settings settings { get; private set; }                  //����â Ŭ����
    [field: SerializeField] public UI_PauseMenu pauseMenu { get; private set; }                 //�Ͻ����� �޴�

    #endregion

    private void Awake()
    {
        SingletonInit();
    }

    private void Start()
    {
        //�÷��̾� ���Ȱ� ���� UI�� �̾��ִ� Presenter Ŭ���� ����
        presenter_PlayerStat = new Presenter_PlayerStat(Player_Controller.Instance, playerStat);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            OnOffUI(playerStatUI.gameObject);
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            OnOffUI(textInteractive.gameObject);
        }
    }

    /// <summary>
    /// ���ӿ�����Ʈ�� Ȱ��ȭ/��Ȱ��ȭ ���θ� �ٲ��� => ���� Ű�� ���
    /// </summary>
    /// <param name="obj"> Ȱ��ȭ, ��Ȱ��ȭ ��ų ���ӿ�����Ʈ </param>
    public void OnOffUI(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(!obj.activeSelf);
    }

    public void OnOffUI(GameObject obj, bool activeSelf)
    {
        if (obj == null) return;

        obj.SetActive(activeSelf);
    }
}
