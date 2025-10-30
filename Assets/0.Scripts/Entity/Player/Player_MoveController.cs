using UnityEngine;

public sealed partial class Player_Controller
{
    /// <summary>
    /// Rigidbody의 velocity를 이용한 이동 메서드
    /// W, A, S, D 키를 이용해 이동 구현, Left Shift를 이용해 달리기 구현
    /// </summary>
    private void Move()
    {
        //이동이 가능한 상태라면
        if (CheckPlayerBehaviorState(PlayerBehaviorState.IsCanMove))
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

            playerRigid.velocity = CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint) ? moveDir * runSpeed : moveDir * moveSpeed;
        }
    }

    /// <summary>
    /// 플레이어가 마우스 포인터를 바라보는 메서드
    /// </summary>
    private void LookAtMousePoint()
    {
        if (CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint)) return;

        Vector3 worldPos = Camera.main.GetMouseWorldPos();
        worldPos.y = transform.position.y;
        Vector3 direction = worldPos - transform.position;

        if (direction.sqrMagnitude > 0.001f)
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

        if (CheckPlayerBehaviorState(PlayerBehaviorState.IsWalk))
        {
            smoothX = Mathf.Lerp(playerAnimator.GetFloat("moveX"), dir.x, 10 * Time.deltaTime);
            smoothZ = Mathf.Lerp(playerAnimator.GetFloat("speed"), CheckPlayerBehaviorState(PlayerBehaviorState.IsSprint) ? dir.z * 2 : dir.z, 10 * Time.deltaTime);
        }

        playerAnimator.SetFloat("moveX", smoothX);
        playerAnimator.SetFloat("speed", smoothZ);
    }
}
