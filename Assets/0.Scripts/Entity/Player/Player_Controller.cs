using System;
using UnityEngine;

[System.Flags]
public enum PlayerBehaviorState : int
{
    None            = 0,
    Alive           = 1 << 0,       //플레이어가 살아있는 상태 = 체력이 0보다 큼
    Dead            = 1 << 1,       //플레이어가 죽은 상태 = 체력이 0 이하
    IsCanMove       = 1 << 2,       //플레이어가 움직일 수 있는 상태
    IsWalk          = 1 << 3,       //플레이어가 걷고 있는 상태
    IsSprint        = 1 << 4,       //플레이어가 달리고 있는 상태
    IsAttack        = 1 << 5,       //플레이어가 공격하고 있는 상태
    IsDodge         = 1 << 6,       //플레이어가 다이빙(구르기) 하는 상태
    IsUsingStamina  = 1 << 7,       //스태미너를 사용 중인지
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
    [SerializeField] private Camera mainCamera;             //메인 카메라
    [SerializeField] private Rigidbody playerRigid;         //플레이어 Rigidbody
    [SerializeField] private Animator playerAnimator;       //플레이어 애니메이터
    [SerializeField] private Transform attackPos;           //플레이어가 공격할 때 공격 탐지를 시작할 위치
    [field: SerializeField] public FieldOfView fieldOfView { get; private set; }       //시야각 구하는 클래스

    [Header("이동 관련 변수")]
    [SerializeField] private float runSpeed;                //플레이어 달리기 속도
    [SerializeField] private float dodgeForce;              //구르기 힘

    [Header("공격 관련 변수")]
    [SerializeField] private AudioClip attackClip;          //어택 사운드
    [SerializeField] private AudioClip hitClip;             //대미지 입을 때 사운드
    [SerializeField] private LayerMask attackMask;          //공격할 대상 레이어
    public event Action playerDeadAction;                   //플레이어가 죽었을 때 발생할 이벤트

    [Header("스태미너 관련 변수")]
    [SerializeField] private float runStamina;              //달리기할 때 소모할 스태미너
    [SerializeField] private float dodgeStamina;            //구르기할 때 소모할 스태미너
    [SerializeField] private float staminaRecoveryDelay;    //몇 초 동안 스태미너를 사용안하면 회복할지
    [SerializeField] private float staminaRecoverySpeed;    //스태미너 회복되는 속도
    [field: SerializeField] public float MaxStamina { get; private set; } = 100f;   //최대 스태미너
    public event Action onStaminaChanged;                   //스태미너 값이 변할 때 실행할 액션 이벤트
    private float staminaRecoveryTimer = 0f;
    private float stamina;
    public float Stamina
    {
        get { return stamina; }
        set
        {
            if(value > MaxStamina)
            {
                value = MaxStamina;
            }
            else if(value < 0)
            {
                value = 0;
            }

            stamina = value;
            onStaminaChanged?.Invoke();
        }
    }

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
        GameManager.Instance.onSceneChanged += Init;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Title) return;
        if (CheckPlayerBehaviorState(PlayerBehaviorState.Dead)) return;


        //공격
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        //마우스 커서 바라보기
        if (!CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint) && !CheckPlayerBehaviorState(PlayerBehaviorState.IsDodge))
        {
            LookAtMousePoint();
        }

        //스태미너 회복
        RecoveryStamina();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Title) return;
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

        stamina = MaxStamina;

        playerBehaviorState = PlayerBehaviorState.Alive | PlayerBehaviorState.IsCanMove;
    }

    public void SetPlayerStat(StatType type, float value)
    {
        stats.SetBaseStat(type, value);
        stats.InitStat(type);
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
