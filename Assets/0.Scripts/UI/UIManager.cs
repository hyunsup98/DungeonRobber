using System.Collections;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region MVP 패턴에 사용하는 변수들

    //플레이어 hp바, 스탯창
    [SerializeField] private UI_PlayerStat playerStat;      //체력바와 스탯창을 모두 관리하는 클래스
    private Presenter_PlayerStat presenter_PlayerStat;

    //설정창
    [field: SerializeField] public UI_Settings settings { get; private set; }       //설정창을 관리하는 클래스
    private Presenter_Settings presenter_Settings;

    #endregion

    #region 단순한 기능이라 직접 관리해줄 변수들

    [field : SerializeField] public UI_InteractiveMessage textInteractive { get; private set; } //상호작용 키 텍스트 클래스
    [field: SerializeField] public UI_PauseMenu pauseMenu { get; private set; }                 //일시정지 메뉴

    #endregion

    private void Awake()
    {
        SingletonInit();
    }

    private void Start()
    {
        //플레이어 스탯과 스탯 UI를 이어주는 Presenter 클래스 생성, 플레이어가 생길 때 만들기
        presenter_PlayerStat = new Presenter_PlayerStat(Player_Controller.Instance, playerStat);
        presenter_Settings = new Presenter_Settings(SoundManager.Instance, settings);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            OnOffUI(textInteractive.gameObject);
        }
    }

    /// <summary>
    /// 게임오브젝트의 활성화/비활성화 여부를 바꿔줌 => 껐다 키는 기능
    /// </summary>
    /// <param name="obj"> 활성화, 비활성화 시킬 게임오브젝트 </param>
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
