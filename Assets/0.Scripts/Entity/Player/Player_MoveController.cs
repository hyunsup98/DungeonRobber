using UnityEngine;

public sealed partial class Player_Controller
{
    /// <summary>
    /// Rigidbody�� velocity�� �̿��� �̵� �޼���
    /// W, A, S, D Ű�� �̿��� �̵� ����, Left Shift�� �̿��� �޸��� ����
    /// </summary>
    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveVelocity = new Vector3(horizontal, 0, vertical).normalized;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            //���� ����Ʈ�� ������ �� - �޸���
            moveVelocity *= runSpeed;
        }
        else
        {
            //���� ����Ʈ�� ������ �ʾ��� �� - �ȱ�
            moveVelocity *= moveSpeed;
        }
        playerRigid.velocity = moveVelocity;
    }

    /// <summary>
    /// �÷��̾ ���콺 �����͸� �ٶ󺸴� �޼���
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
