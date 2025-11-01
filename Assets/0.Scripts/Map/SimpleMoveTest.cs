using UnityEngine;

// ������ �÷��̾� �̵� ��ũ��Ʈ
public class SimpleMoveTest : MonoBehaviour
{
    // �̵� �ӵ�
    public float moveSpeed = 5f;

    // Update�� �� �����Ӹ��� ȣ���
    void Update()
    {
        // �Է°� �ޱ� (WASD �Ǵ� ����Ű)
        float h = Input.GetAxis("Horizontal");  // A, D �Ǵ� ��, ��
        float v = Input.GetAxis("Vertical");    // W, S �Ǵ� ��, ��

        // �̵� ���� ���� ���
        Vector3 moveDir = new Vector3(h, 0, v);

        // ����ȭ�ؼ� �밢�� �̵� �ӵ� ����
        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        // Transform ��� �̵�
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        // �̵� �������� ȸ�� (�̵� ���� ����)
        if (moveDir != Vector3.zero)
            transform.forward = moveDir;
    }
}
