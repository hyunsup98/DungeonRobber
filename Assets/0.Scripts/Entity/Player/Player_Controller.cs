using System;
using System.Collections;
using Unity.VisualScripting;
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
    IsDoingUpperBody = 1 << 5,  //플레이어가 상체가 하는 동작을 하고 있는지에 대한 상태 - 공격, 먹기, 마시기, 상자 열기 등의 모션
}

/// <summary>
/// 플레이어에 관련된 기능들을 담당하는 클래스
/// 이동, 공격, 애니메이션 등의 요소를 담당
/// </summary>
public sealed partial class Player_Controller : Entity
{
    [SerializeField] private BaseStat stats;
    private BuffManager buffManager;

    [SerializeField] private BaseBuff freeze;

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

    public float CurrentHP                              //현재 체력 프로퍼티
    {
        get { return currentHP; }
        set
        {
            if(value > maxHP)
            {
                value = maxHP;
            }
            else if(value < 0)
            {
                value = 0;
            }

            currentHP = value;

            //플레이어의 체력이 0 이하면 PlayerDeadAction 호출
            if(currentHP <= 0)
            {
                playerDeadAction?.Invoke();
            }
        }
    }

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (playerRigid == null && TryGetComponent<Rigidbody>(out var rigid))
            playerRigid = rigid;

        if(playerAnimator == null && TryGetComponent<Animator>(out var anim))
            playerAnimator = anim;

        if (stats == null)
            stats = new BaseStat();

        if (buffManager == null)
            buffManager = new BuffManager();

        Init();
        stats.Init();
    }

    private void Start()
    {
        playerDeadAction += Dead;
    }

    private void Update()
    {
        if (CheckPlayerBehaviorState(PlayerBehaviorState.Dead)) return;

        //마우스 커서 바라보기
        LookAtMousePoint();

        //공격
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        //Test
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log(stats.GetStat(StatType.HP));
            Debug.Log(stats.GetStat(StatType.MoveSpeed));
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            ApplyBuffToEntity(stats, freeze);
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

        if (runSpeed < moveSpeed)
            runSpeed = moveSpeed * 1.5f;

        playerBehaviorState = PlayerBehaviorState.Alive | PlayerBehaviorState.IsCanMove;
    }

    //todo, 상체 행동 애니메이션 레이어 보간 작업인데 원하는 방향으로 안나와서 잠시 보류
    private IEnumerator PlayOtherAnimatorLayer(string motionName)
    {
        playerAnimator.SetTrigger(motionName);
        playerAnimator.SetLayerWeight(1, 1);
        AddPlayerBehaviorState(PlayerBehaviorState.IsDoingUpperBody);

        while (true)
        {
            if(playerAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
            {
                playerAnimator.SetLayerWeight(1, 1 - playerAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime);
            }
            else
            {
                RemovePlayerBehaviorState(PlayerBehaviorState.IsDoingUpperBody);
                break;
            }

            yield return null;
        }
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
    /// <returns></returns>
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
    }

    //버프 테스트
    private void ApplyBuffToEntity(BaseStat stat, params BaseBuff[] buffs)
    {
        foreach(var buff in buffs)
        {
            buffManager.ApplyBuff(buff, stat);
        }
    }
}
