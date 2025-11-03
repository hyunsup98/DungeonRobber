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

    /// <summary>
    /// 퀵슬롯 입력 처리 (1~6번 키)
    /// </summary>
    private void HandleQuickSlotInput()
    {
        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (quickSlots != null)
                {
                    Item item = quickSlots.GetItem(i);
                    if (item != null)
                    {
                        UseItem(item, i);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    /// <param name="item">사용할 아이템</param>
    /// <param name="slotIndex">퀵슬롯 인덱스</param>
    private void UseItem(Item item, int slotIndex)
    {
        if (item == null || quickSlots == null)
            return;

        Debug.Log($"아이템 사용 시도: {item.itemName}");

        // 소비형 아이템 처리
        if (item.itemType == Item.ItemType.Consumable)
        {
            // 빙결 수류탄 특별 처리
            if (item.itemName == "빙결 수류탄")
            {
                ThrowFreezeGrenade(item, slotIndex);
                return;
            }

            // 일반 아이템 버프 적용
            if (item.useBuff != null)
            {
                try
                {
                    // Player_Controller의 stats에 버프 적용 (Entity에서 상속받은 stats)
                    ApplyBuffToEntity(item.useBuff);
                    Debug.Log($"'{item.itemName}' 버프 효과가 적용되었습니다!");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"'{item.itemName}' 버프 적용 중 오류 발생: {e.Message}");
                }
            }
            else
            {
                Debug.Log($"'{item.itemName}' 아이템을 사용했습니다! (버프 효과 없음)");
            }

            // 인벤토리 수량 감소
            if (inventory != null)
            {
                inventory.DecreaseItemQuantity(item);

                // 인벤토리 수량 확인
                uint remainingQuantity = inventory.GetItemQuantity(item);

                if (remainingQuantity == 0)
                {
                    // 수량이 0이 되면 퀵슬롯에서도 제거
                    quickSlots.RemoveItem(slotIndex);
                }
                else
                {
                    // 수량이 남아있으면 퀵슬롯 수량 동기화
                    quickSlots.RefreshSlots();
                }
            }
        }
        else if (item.itemType == Item.ItemType.Equipment)
        {
            // TODO: 장비 장착 로직
            Debug.Log($"'{item.itemName}' 장비를 장착했습니다!");
        }
        else
        {
            Debug.Log($"'{item.itemName}'는 사용할 수 없는 아이템입니다.");
        }
    }

    /// <summary>
    /// 빙결 수류탄 던지기
    /// </summary>
    private void ThrowFreezeGrenade(Item item, int slotIndex)
    {
        // 마우스 위치로 레이캐스트
        Ray ray = mainCamera != null ? mainCamera.ScreenPointToRay(Input.mousePosition) : Camera.main.ScreenPointToRay(Input.mousePosition);
        UnityEngine.RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            Vector3 throwPosition = hit.point;

            // 범위 내의 적 찾기
            Collider[] colliders = Physics.OverlapSphere(throwPosition, 3f); // 3미터 반경

            bool hitEnemy = false;
            foreach (var col in colliders)
            {
                Monster enemy = col.GetComponent<Monster>();
                if (enemy != null)
                {
                    // 빙결 버프 적용
                    if (item.useBuff != null)
                    {
                        enemy.ApplyBuffToEntity(item.useBuff);
                        hitEnemy = true;
                        Debug.Log($"'{enemy.name}'에게 빙결 수류탄이 맞았습니다!");
                    }
                }
            }

            if (!hitEnemy)
            {
                Debug.Log("빙결 수류탄이 적에게 맞지 않았습니다.");
            }

            // 인벤토리 수량 감소
            if (inventory != null)
            {
                inventory.DecreaseItemQuantity(item);

                uint remainingQuantity = inventory.GetItemQuantity(item);

                if (remainingQuantity == 0)
                {
                    quickSlots.RemoveItem(slotIndex);
                }
                else
                {
                    quickSlots.RefreshSlots();
                }
            }
        }
    }

    /// <summary>
    /// 아이템 버프 적용 (컨텍스트 메뉴에서 호출)
    /// </summary>
    /// <param name="buff">적용할 버프</param>
    public void ApplyItemBuff(BaseBuff buff)
    {
        if (buff != null)
        {
            try
            {
                // Player_Controller의 stats에 버프 적용 (Entity에서 상속받은 stats)
                ApplyBuffToEntity(buff);
                Debug.Log($"버프 효과가 적용되었습니다!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"버프 적용 중 오류 발생: {e.Message}");
            }
        }
    }
    
    /// <summary>
    /// 맵에서 아이템 줍기 시도
    /// </summary>
    private void TryPickupItem()
    {
        // 카메라에서 마우스 위치로 레이캐스트
        Ray ray = mainCamera != null ? mainCamera.ScreenPointToRay(Input.mousePosition) : Camera.main.ScreenPointToRay(Input.mousePosition);
        UnityEngine.RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            GroundItem groundItem = hit.collider.GetComponent<GroundItem>();
            
            if (groundItem != null)
            {
                // 줍기 가능한 거리인지 확인
                if (groundItem.CanPickup(transform.position))
                {
                    Item pickedItem = groundItem.Pickup();
                    
                    if (pickedItem != null && inventory != null)
                    {
                        if (inventory.AddItem(pickedItem))
                        {
                            Debug.Log($"'{pickedItem.itemName}' 아이템을 획득했습니다!");
                        }
                        else
                        {
                            Debug.LogWarning("인벤토리가 가득 찼습니다.");
                            // 아이템 다시 활성화
                            groundItem.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    Debug.Log("아이템이 너무 멀리 있습니다.");
                }
            }
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
            if (foundChest != null)
            {
                Debug.Log("[상자] E키를 눌러 상자를 열 수 있습니다.");
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
            Debug.Log("상자를 열었습니다!");
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
