using System;
using System.Collections;
using Unity.VisualScripting;
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
    IsDoingUpperBody = 1 << 5,  //�÷��̾ ��ü�� �ϴ� ������ �ϰ� �ִ����� ���� ���� - ����, �Ա�, ���ñ�, ���� ���� ���� ���
}

/// <summary>
/// �÷��̾ ���õ� ��ɵ��� ����ϴ� Ŭ����
/// �̵�, ����, �ִϸ��̼� ���� ��Ҹ� ���
/// </summary>
public sealed partial class Player_Controller : Entity
{
    [SerializeField] private BaseStat stats;
    private BuffManager buffManager;

    [SerializeField] private BaseBuff freeze;

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

    public float CurrentHP                              //���� ü�� ������Ƽ
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

            //�÷��̾��� ü���� 0 ���ϸ� PlayerDeadAction ȣ��
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

        //���콺 Ŀ�� �ٶ󺸱�
        LookAtMousePoint();

        //����
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

        if (runSpeed < moveSpeed)
            runSpeed = moveSpeed * 1.5f;

        playerBehaviorState = PlayerBehaviorState.Alive | PlayerBehaviorState.IsCanMove;
    }

    //todo, ��ü �ൿ �ִϸ��̼� ���̾� ���� �۾��ε� ���ϴ� �������� �ȳ��ͼ� ��� ����
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

    //���� �׽�Ʈ
    private void ApplyBuffToEntity(BaseStat stat, params BaseBuff[] buffs)
    {
        foreach(var buff in buffs)
        {
            buffManager.ApplyBuff(buff, stat);
        }
    }
}
