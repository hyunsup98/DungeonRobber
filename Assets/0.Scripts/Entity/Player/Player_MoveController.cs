using UnityEngine;

public sealed partial class Player_Controller
{
    /// <summary>
    /// Rigidbody�� velocity�� �̿��� �̵� �޼���
    /// W, A, S, D Ű�� �̿��� �̵� ����, Left Shift�� �̿��� �޸��� ����
    /// </summary>
    private void Move()
    {
        //�̵��� ������ ���¶��
        if(CheckPlayerBehaviorState(PlayerBehaviorState.IsCanMove))
        if(CheckPlayerBehaviorState(PlayerBehaviorState.IsCanMove))
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 moveDir = new Vector3(horizontal, 0, vertical).normalized;

            //�Է� Ű ���Ͱ��� ���� �Ȱ� �ִ� �������� �ƴ��� ����
            if (moveDir.sqrMagnitude < 0.005f)
            {
                RemovePlayerBehaviorState(PlayerBehaviorState.IsWalk);
            }
            else
            {
                AddPlayerBehaviorState(PlayerBehaviorState.IsWalk);
            }

            if(CheckPlayerBehaviorState(PlayerBehaviorState.IsWalk))
            if(CheckPlayerBehaviorState(PlayerBehaviorState.IsWalk))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    //���� ����Ʈ�� ������ �� - �޸���
                    AddPlayerBehaviorState(PlayerBehaviorState.IsSprint);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), 10 * Time.deltaTime);
                }
                else
                {
                    //���� ����Ʈ�� ������ �ʾ��� �� - �ȱ�
                    RemovePlayerBehaviorState(PlayerBehaviorState.IsSprint);
                }
            }

            //�Է¹��� moveDir(������ǥ ����)�� ������ǥ ���� ���� ���ͷ� ��ȯ
            Vector3 localMoveDir = transform.InverseTransformDirection(moveDir).normalized;
            SetMoveAnimationBlend(localMoveDir);

            playerRigid.velocity = CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint) ? moveDir * runSpeed : moveDir * stats.GetStat(StatType.MoveSpeed);
        }
    }

    /// <summary>
    /// �÷��̾ ���콺 �����͸� �ٶ󺸴� �޼���
    /// </summary>
    private void LookAtMousePoint()
    {
        Vector3 worldPos = Camera.main.GetWorldPosToMouse();

        Vector3 direction = worldPos - transform.position;
        direction.y = transform.position.y;
        direction.y = transform.position.y;

        if(direction.sqrMagnitude > 0.001f)
        if(direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    /// <summary>
    /// �÷��̾��� �̵� �ִϸ��̼� �������� ���� ó���ϴ� �޼���
    /// </summary>
    /// <param name="dir"> �÷��̾��� forward ���� �̵��ϴ� ���� ���� </param>
    private void SetMoveAnimationBlend(Vector3 dir)
    {
        float smoothX = 0;
        float smoothZ = 0;

        if(CheckPlayerBehaviorState(PlayerBehaviorState.IsWalk))
        if(CheckPlayerBehaviorState(PlayerBehaviorState.IsWalk))
        {
            smoothX = Mathf.Lerp(playerAnimator.GetFloat("moveX"), dir.x, 10 * Time.deltaTime);
            smoothZ = Mathf.Lerp(playerAnimator.GetFloat("speed"), CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint) ? dir.z * 2 : dir.z, 10 * Time.deltaTime);
        }

        playerAnimator.SetFloat("moveX", smoothX);
        playerAnimator.SetFloat("speed", smoothZ);
    }
}
