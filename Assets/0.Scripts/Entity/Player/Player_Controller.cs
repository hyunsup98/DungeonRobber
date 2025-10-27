using UnityEngine;

/// <summary>
/// 플레이어에 관련된 기능들을 담당하는 클래스
/// 이동, 공격, 애니메이션 등의 요소를 담당
/// </summary>
public sealed partial class Player_Controller : Entity
{
    [Header("컴포넌트 변수")]
    [SerializeField] private Camera mainCamera;         //메인 카메라
    [SerializeField] private Rigidbody playerRigid;     //플레이어 Rigidbody
    [SerializeField] private Animator playerAnimator;   //플레이어 애니메이터
    [SerializeField] private Transform attackPos;       //플레이어가 공격할 때 공격 탐지를 시작할 위치

    [Header("이동 관련 변수")]
    [SerializeField] private float runSpeed;            //플레이어 달리기 속도

    [Header("공격 관련 변수")]
    [SerializeField] private LayerMask attackMask;      //공격할 대상 레이어

    private void Awake()
    {
        if (playerRigid == null && TryGetComponent<Rigidbody>(out var rigid))
            playerRigid = rigid;

        if (mainCamera == null)
            mainCamera = Camera.main;

        Init();
    }

    public float CurrentHP      //현재 체력 프로퍼티
    {
        get { return currentHP; }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            else if(value > maxHP)
            {
                value = maxHP;
            }
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        LookAtMousePoint();
    }

    private void FixedUpdate()
    {
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
    }
}
