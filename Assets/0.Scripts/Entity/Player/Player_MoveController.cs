using System.Collections;
using UnityEngine;

public sealed partial class Player_Controller
{
    public float GetPlayerCurrentSpeed()
    {
        return CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint) ? runSpeed : stats.GetStat(StatType.MoveSpeed);
    }

    /// <summary>
    /// Rigidbody의 velocity를 이용한 이동 메서드
    /// W, A, S, D 키를 이용해 이동 구현, Left Shift를 이용해 달리기 구현
    /// </summary>
    private void Move()
    {
        //이동이 가능한 상태라면
        if(CheckPlayerBehaviorState(PlayerBehaviorState.IsCanMove))
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 moveDir = new Vector3(horizontal, 0, vertical).normalized;

            //입력 키 벡터값에 따라서 걷고 있는 상태인지 아닌지 세팅
            if (moveDir.sqrMagnitude < 0.005f)
            {
                RemovePlayerBehaviorState(PlayerBehaviorState.IsWalk);
            }
            else
            {
                AddPlayerBehaviorState(PlayerBehaviorState.IsWalk);
            }

            //구르기
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
                    //왼쪽 쉬프트를 눌렀을 때 - 달리기
                    AddPlayerBehaviorState(PlayerBehaviorState.IsSprint);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), 10 * Time.deltaTime);
                }
                else
                {
                    //왼쪽 쉬프트를 누르지 않았을 때 - 걷기
                    RemovePlayerBehaviorState(PlayerBehaviorState.IsSprint);
                }
            }

            //입력받은 moveDir(월드좌표 기준)을 로컬좌표 기준 방향 벡터로 변환
            Vector3 localMoveDir = transform.InverseTransformDirection(moveDir).normalized;
            SetMoveAnimationBlend(localMoveDir);

            playerRigid.velocity = CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint) ? moveDir * runSpeed : moveDir * stats.GetStat(StatType.MoveSpeed);
        }
    }

    /// <summary>
    /// 플레이어가 마우스 포인터를 바라보는 메서드
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
    /// 플레이어의 이동 애니메이션 블렌딩을 보간 처리하는 메서드
    /// </summary>
    /// <param name="dir"> 플레이어의 forward 기준 이동하는 방향 벡터 </param>
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
        //상태 플래그 변경 및 구르기 애니메이션 적용
        AddPlayerBehaviorState(PlayerBehaviorState.IsDodge);
        RemovePlayerBehaviorState(PlayerBehaviorState.IsCanMove);
        playerAnimator.SetTrigger("dodge");

        //플레이어 구르기 물리 적용
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
