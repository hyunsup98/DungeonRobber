using UnityEngine;

public sealed partial class Player_Controller
{
    /// <summary>
    /// Rigidbody의 velocity를 이용한 이동 메서드
    /// W, A, S, D 키를 이용해 이동 구현, Left Shift를 이용해 달리기 구현
    /// </summary>
    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveVelocity = new Vector3(horizontal, 0, vertical).normalized;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            //왼쪽 쉬프트를 눌렀을 때 - 달리기
            moveVelocity *= runSpeed;
        }
        else
        {
            //왼쪽 쉬프트를 누르지 않았을 때 - 걷기
            moveVelocity *= moveSpeed;
        }
        playerRigid.velocity = moveVelocity;
    }

    /// <summary>
    /// 플레이어가 마우스 포인터를 바라보는 메서드
    /// </summary>
    private void LookAtMousePoint()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.transform.position.y;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        Vector3 direction = worldPos - transform.position;
        direction.y = 0f;

        if(direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

}
