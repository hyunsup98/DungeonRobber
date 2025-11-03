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

    [Header("인벤토리 및 퀵슬롯")]
    [SerializeField] private Inventory inventory;            //인벤토리
    [SerializeField] private QuickSlot_Controller quickSlots; //퀵슬롯
    
    [Header("아이템 컨트롤러")]
    [SerializeField] private Item_Controller itemController; //아이템 관련 기능 관리
    
    [Header("상점")]
    [SerializeField] private Shop shop;                      //상점
    
    [Header("상점 NPC 감지")]
    [SerializeField] private float interactionRange = 3f;    //상점 NPC와의 상호작용 거리
    
    private GameObject currentNearbyShop = null;
    
    [Header("상자 감지")]
    private TreasureChest currentNearbyChest = null;

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

        // Item_Controller가 없으면 자동으로 찾기
        if (itemController == null)
            itemController = FindObjectOfType<Item_Controller>();

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

        // 퀵슬롯 아이템 사용 (1~6번 키)
        if (itemController != null)
        {
            itemController.HandleQuickSlotInput();
        }
        
        // 마우스 우클릭 (아이템 획득)
        if (Input.GetMouseButtonDown(1))
        {
            if (itemController != null)
            {
                itemController.TryPickupItem(transform.position);
            }
        }
        
        // E 키 (인벤토리 열기/닫기)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inventory != null)
                inventory.ToggleInventory();
        }
        
        // Q 키 (상점 열기/닫기)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentNearbyShop != null && shop != null)
            {
                // 상점 열림/닫힘 토글
                if (!shop.IsOpen)
                {
                    shop.ShowShop();
                    if (inventory != null && !inventory.IsOpen)
                        inventory.ShowInventory();
                }
                else
                {
                    shop.HideShop();
                    // 인벤토리는 상점이 닫힐 때만 닫음 (Tab으로도 닫을 수 있음)
                }
            }
            else if (shop != null && shop.IsOpen)
            {
                // 상점이 열려있고 NPC 근처가 아니면 상점만 닫기
                shop.HideShop();
            }
        }
        
        // 상점 NPC 감지
        CheckForNearbyShop();
        
        // 상자 감지 및 상호작용
        CheckForNearbyChest();
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteractWithChest();
        }
        
        // F 키 (상자 열기 우선, 없으면 아이템 줍기)
        // NPC 상호작용은 NPC 클래스에서 자체적으로 처리하므로, 여기서는 상자/아이템만 처리
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 상자 근처에 있으면 상자 열기 (우선순위)
            if (currentNearbyChest != null && !currentNearbyChest.IsOpened)
            {
                TryInteractWithChest();
            }
            // 상자가 없으면 아이템 줍기 시도
            else if (itemController != null)
            {
                itemController.TryPickupNearbyItem(transform.position);
            }
        }
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

    /// <summary>
    /// 아이템 버프 적용 (컨텍스트 메뉴에서 호출)
    /// </summary>
    /// <param name="buff">적용할 버프</param>
    public void ApplyItemBuff(BaseBuff buff)
    {
        if (itemController != null)
        {
            itemController.ApplyItemBuff(buff);
        }
    }
    
    /// <summary>
    /// 근처에 상점 NPC가 있는지 확인
    /// </summary>
    private void CheckForNearbyShop()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
        
        GameObject foundShop = null;
        foreach (var col in colliders)
        {
            if (col.CompareTag("Shopper"))
            {
                foundShop = col.gameObject;
                break;
            }
        }

        if (foundShop != currentNearbyShop)
        {
            currentNearbyShop = foundShop;
            if (foundShop != null)
            {
                Debug.Log("[상점] NPC 접촉: Q키를 눌러 상점을 여십시오.");
            }
            else if (shop != null && shop.IsOpen)
            {
                // NPC에서 멀어지면 상점 자동 닫기
                shop.HideShop();
                if (inventory != null && inventory.IsOpen)
                    inventory.HideInventory();
            }
        }
    }
    
    /// <summary>
    /// 근처에 상자가 있는지 확인
    /// </summary>
    private void CheckForNearbyChest()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
        
        TreasureChest foundChest = null;
        foreach (var col in colliders)
        {
            TreasureChest chest = col.GetComponent<TreasureChest>();
            if (chest != null && !chest.IsOpened && chest.CanInteract(transform.position))
            {
                foundChest = chest;
                break;
            }
        }

        if (foundChest != currentNearbyChest)
        {
            currentNearbyChest = foundChest;
            
            // UI 텍스트 표시/숨김
            if (UIManager.Instance != null && UIManager.Instance.textInteractive != null)
            {
                if (foundChest != null)
                {
                    // 상자가 근처에 있으면 텍스트 표시
                    UIManager.Instance.textInteractive.OnInteractiveMessage("F키를 눌러 상자를 열 수 있습니다");
                }
                else
                {
                    // 상자가 없으면 텍스트 숨김
                    UIManager.Instance.textInteractive.OffInteractiveMessage();
                }
            }
        }
    }
    
    /// <summary>
    /// 상자와 상호작용 시도
    /// </summary>
    private void TryInteractWithChest()
    {
        if (currentNearbyChest != null)
        {
            currentNearbyChest.Open();
            
            if (UIManager.Instance != null && UIManager.Instance.textInteractive != null)
            {
                UIManager.Instance.textInteractive.OffInteractiveMessage();
            }
            currentNearbyChest = null;
        }
    }
    
    // 디버그용 기즈모 그리기
    private void OnDrawGizmos()
    {
        if (currentNearbyShop != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
