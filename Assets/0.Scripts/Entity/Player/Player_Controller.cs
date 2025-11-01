using System;
using UnityEngine;

[System.Flags]
public enum PlayerBehaviorState : int
{
    None        = 0,
    Alive       = 1 << 0,       //�÷��̾ ����ִ� ���� = ü���� 0���� ŭ
    Dead        = 1 << 1,       //�÷��̾ ���� ���� = ü���� 0 ����
    IsCanMove   = 1 << 2,       //�÷��̾ ������ �� �ִ� ����
    IsWalk      = 1 << 3,       //�÷��̾ �Ȱ� �ִ� ����
    IsSprint    = 1 << 4,       //�÷��̾ �޸��� �ִ� ����
    IsAttack    = 1 << 5,       //�÷��̾ �����ϰ� �ִ� ����
}

/// <summary>
/// �÷��̾ ���õ� ��ɵ��� ����ϴ� Ŭ����
/// �̵�, ����, �ִϸ��̼� ���� ��Ҹ� ���
/// </summary>
public sealed partial class Player_Controller : Entity
{
    public static Player_Controller Instance { get; private set; }  //�̹� Entity Ŭ������ ��ӹް� �ֱ⿡ ���� �������

    public event Action onPlayerStatChanged;

    [Header("������Ʈ ����")]
    [SerializeField] private Camera mainCamera;         //���� ī�޶�
    [SerializeField] private Rigidbody playerRigid;     //�÷��̾� Rigidbody
    [SerializeField] private Animator playerAnimator;   //�÷��̾� �ִϸ�����
    [SerializeField] private Transform attackPos;       //�÷��̾ ������ �� ���� Ž���� ������ ��ġ

    [Header("�̵� ���� ����")]
    [SerializeField] private float runSpeed;            //�÷��̾� �޸��� �ӵ�

    [Header("���� ���� ����")]
    [SerializeField] private LayerMask attackMask;      //������ ��� ���̾�
    public event Action playerDeadAction;               //�÷��̾ �׾��� �� �߻��� �̺�Ʈ

    private PlayerBehaviorState playerBehaviorState;    //�÷��̾� �ൿ�� ���õ� ���� �÷���

    //�÷��̾��� ������ �ܺο��� �޾ƿ��� �޼���
    public BaseStat GetPlayerStat() => stats;

    private void Awake()
    {
        #region �̱���
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

        //����
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        //���콺 Ŀ�� �ٶ󺸱�
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

        //�̵�
        Move();
    }

    /// <summary>
    /// �ʱ�ȭ �޼���
    /// �÷��̾� �̵��ӵ� �� �ʱ�ȭ
    /// </summary>
    protected override void Init()
    {
        base.Init();

        if (runSpeed < stats.GetStat(StatType.MoveSpeed))
            runSpeed = stats.GetStat(StatType.MoveSpeed) * 1.5f;

        playerBehaviorState = PlayerBehaviorState.Alive | PlayerBehaviorState.IsCanMove;
    }

    #region �÷��̾��� �ൿ ���� ����
    /// <summary>
    /// �÷��̾� �ൿ ���� �÷��׿� ���ο� ���� �߰�
    /// </summary>
    /// <param name="state"> �߰��� ���� </param>
    private void AddPlayerBehaviorState(PlayerBehaviorState state)
    {
        playerBehaviorState |= state;
    }

    /// <summary>
    /// �÷��̾� �ൿ ���� �÷��׿� �ִ� ���� ����
    /// </summary>
    /// <param name="state"> ������ ���� </param>
    private void RemovePlayerBehaviorState(PlayerBehaviorState state)
    {
        playerBehaviorState &= ~state;
    }

    /// <summary>
    /// �÷��̾� �ൿ ���� �÷��׿� ���°� Ȱ��ȭ�Ǿ� �ִ��� üũ
    /// </summary>
    /// <param name="states"> üũ�� ���� ��� </param>
    /// <returns> True = �ش� ��Ʈ �÷��װ� 1�� ��� / False = �ش� ��Ʈ �÷��װ� 0�� ��� </returns>
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
