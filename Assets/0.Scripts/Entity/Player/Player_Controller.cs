using UnityEngine;

/// <summary>
/// �÷��̾ ���õ� ��ɵ��� ����ϴ� Ŭ����
/// �̵�, ����, �ִϸ��̼� ���� ��Ҹ� ���
/// </summary>
public sealed partial class Player_Controller : Entity
{
    [Header("������Ʈ ����")]
    [SerializeField] private Camera mainCamera;         //���� ī�޶�
    [SerializeField] private Rigidbody playerRigid;     //�÷��̾� Rigidbody
    [SerializeField] private Animator playerAnimator;   //�÷��̾� �ִϸ�����
    [SerializeField] private Transform attackPos;       //�÷��̾ ������ �� ���� Ž���� ������ ��ġ

    [Header("�̵� ���� ����")]
    [SerializeField] private float runSpeed;            //�÷��̾� �޸��� �ӵ�

    [Header("���� ���� ����")]
    [SerializeField] private LayerMask attackMask;      //������ ��� ���̾�

    private void Awake()
    {
        if (playerRigid == null && TryGetComponent<Rigidbody>(out var rigid))
            playerRigid = rigid;

        if (mainCamera == null)
            mainCamera = Camera.main;

        Init();
    }

    public float CurrentHP      //���� ü�� ������Ƽ
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
    /// �ʱ�ȭ �޼���
    /// �÷��̾� �̵��ӵ� �� �ʱ�ȭ
    /// </summary>
    protected override void Init()
    {
        base.Init();

        if (runSpeed < moveSpeed)
            runSpeed = moveSpeed * 1.5f;
    }
}
