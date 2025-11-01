using System;
using UnityEngine;

[System.Flags]
public enum PlayerBehaviorState : int
{
    None        = 0,
    Alive       = 1 << 0,       //플레이어가 살아있는 상태 = 체력이 0보다 큼
    Dead        = 1 << 1,       //플레이어가 죽은 상태 = 체력이 0 이하
    IsCanMove   = 1 << 2,       //플레이어가 움직일 수 있는 상태
    IsWalk      = 1 << 3,       //플레이어가 걷고 있는 상태
    IsSprint    = 1 << 4,       //플레이어가 달리고 있는 상태
    IsAttack    = 1 << 5,       //플레이어가 공격하고 있는 상태
}

/// <summary>
/// 플레이어에 관련된 기능들을 담당하는 클래스
/// 이동, 공격, 애니메이션 등의 요소를 담당
/// </summary>
public sealed partial class Player_Controller : Entity
{
    public static Player_Controller Instance { get; private set; }  //이미 Entity 클래스를 상속받고 있기에 따로 만들어줌

    public event Action onPlayerStatChanged;

    [Header("컴포넌트 변수")]
    [SerializeField] private Camera mainCamera;         //메인 카메라
    [SerializeField] private Rigidbody playerRigid;     //플레이어 Rigidbody
    [SerializeField] private Animator playerAnimator;   //플레이어 애니메이터
    [SerializeField] private Transform attackPos;       //플레이어가 공격할 때 공격 탐지를 시작할 위치

    [Header("이동 관련 변수")]
    [SerializeField] private float runSpeed;            //플레이어 달리기 속도

    [Header("공격 관련 변수")]
    [SerializeField] private LayerMask attackMask;      //공격할 대상 레이어
    public event Action playerDeadAction;               //플레이어가 죽었을 때 발생할 이벤트

    private PlayerBehaviorState playerBehaviorState;    //플레이어 행동에 관련된 상태 플래그

    //플레이어의 스탯을 외부에서 받아오는 메서드
    public BaseStat GetPlayerStat() => stats;

    private void Awake()
    {
        #region 싱글턴
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (playerRigid == null && TryGetComponent<Rigidbody>(out var rigid))
            playerRigid = rigid;

        if(playerAnimator == null && TryGetComponent<Animator>(out var anim))
            playerAnimator = anim;

        Init();
    }

    private void Start()
    {
        playerDeadAction += Dead;
    }

    private void Update()
    {
        if (CheckPlayerBehaviorState(PlayerBehaviorState.Dead)) return;

        //공격
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        //마우스 커서 바라보기
        if(!CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint))
        {
            LookAtMousePoint();
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            GetDamage(5f);
        }
    }

    private void FixedUpdate()
    {
        if (CheckPlayerBehaviorState(PlayerBehaviorState.Dead)) return;

        //이동
        Move();
    }

    /// <summary>
    /// 초기화 메서드
    /// 플레이어 이동속도 등 초기화
    /// </summary>
    protected override void Init()
    {
        base.Init();

        if (runSpeed < stats.GetStat(StatType.MoveSpeed))
            runSpeed = stats.GetStat(StatType.MoveSpeed) * 1.5f;

        playerBehaviorState = PlayerBehaviorState.Alive | PlayerBehaviorState.IsCanMove;
    }

    #region 플레이어의 행동 상태 제어
    /// <summary>
    /// 플레이어 행동 상태 플래그에 새로운 상태 추가
    /// </summary>
    /// <param name="state"> 추가할 상태 </param>
    private void AddPlayerBehaviorState(PlayerBehaviorState state)
    {
        playerBehaviorState |= state;
    }

    /// <summary>
    /// 플레이어 행동 상태 플래그에 있는 상태 제거
    /// </summary>
    /// <param name="state"> 삭제할 상태 </param>
    private void RemovePlayerBehaviorState(PlayerBehaviorState state)
    {
        playerBehaviorState &= ~state;
    }

    /// <summary>
    /// 플레이어 행동 상태 플래그에 상태가 활성화되어 있는지 체크
    /// </summary>
    /// <param name="states"> 체크할 상태 목록 </param>
    /// <returns> True = 해당 비트 플래그가 1일 경우 / False = 해당 비트 플래그가 0일 경우 </returns>
    private bool CheckPlayerBehaviorState(params PlayerBehaviorState[] states)
    {
        foreach(var state in states)
        {
            if((playerBehaviorState & state) == 0)
            {
                return false;
            }
        }

        return true;
    }
    #endregion

    private void OnDestroy()
    {
        playerDeadAction -= Dead;

        StopAllCoroutines();
    }
}
