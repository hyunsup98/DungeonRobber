using System.Collections;
using Unity.VisualScripting;
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

            //������
            if (Input.GetKey(KeyCode.Space))
            {
                if (!CheckPlayerBehaviorState(PlayerBehaviorState.IsDodge))
                {
                    StartCoroutine(Dive(moveDir));
                    return;
                }
            }

            if (CheckPlayerBehaviorState(PlayerBehaviorState.IsWalk))
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
        Vector3 worldPos = Camera.main.GetWorldPosToMouse(transform);

        Vector3 direction = worldPos - transform.position;
        direction.y = 0f;

        if(direction.sqrMagnitude > 0.005f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    /// <summary>
    /// �÷��̾��� �̵� �ִϸ��̼� ������ ���� ó���ϴ� �޼���
    /// </summary>
    /// <param name="dir"> �÷��̾��� forward ���� �̵��ϴ� ���� ���� </param>
    private void SetMoveAnimationBlend(Vector3 dir)
    {
        float smoothX = 0;
        float smoothZ = 0;

        if(CheckPlayerBehaviorState(PlayerBehaviorState.IsWalk))
        {
            smoothX = Mathf.Lerp(playerAnimator.GetFloat("moveX"), dir.x, 10 * Time.deltaTime);
            smoothZ = Mathf.Lerp(playerAnimator.GetFloat("speed"), CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint) ? dir.z * 2 : dir.z, 10 * Time.deltaTime);
        }

        playerAnimator.SetFloat("moveX", smoothX);
        playerAnimator.SetFloat("speed", smoothZ);
    }

    private IEnumerator Dive(Vector3 moveDir)
    {
        //���� �÷��� ���� �� ������ �ִϸ��̼� ����
        AddPlayerBehaviorState(PlayerBehaviorState.IsDodge);
        RemovePlayerBehaviorState(PlayerBehaviorState.IsCanMove);
        playerAnimator.SetTrigger("dodge");

        //�÷��̾� ������ ���� ����
        if(moveDir.sqrMagnitude <= 0.0005f)
        {
            playerRigid.AddForce(transform.forward * dodgeForce, ForceMode.Impulse);
        }
        else
        {
            playerRigid.velocity = Vector3.zero;
            transform.rotation = Quaternion.LookRotation(moveDir);
            playerRigid.AddForce(moveDir * dodgeForce, ForceMode.Impulse);
        }

        yield return new WaitUntil(() => 
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dodge") && 
            playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
            );
        yield return CoroutineManager.waitForSeconds(0.1f);

        RemovePlayerBehaviorState(PlayerBehaviorState.IsDodge);
        AddPlayerBehaviorState(PlayerBehaviorState.IsCanMove);
    }
}
