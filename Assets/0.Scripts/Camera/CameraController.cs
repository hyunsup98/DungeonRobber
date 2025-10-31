using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    //�ٶ� Ÿ��
    [SerializeField] private Transform target;

    //ī�޶� �� ���� y��ǥ ��
    [SerializeField] private float yOffset = 20f;

    //���ͺ並 ���� �÷��̾� ��ġ���� z��ǥ�� �󸶳� �Ʒ��� �������� ���� ��
    [SerializeField] private float zOffset = -10f;

    private void Awake()
    {
        SingletonInit();
    }

    private void LateUpdate()
    {
        FollowToTarget();
        RotateToTarget();
    }

    //Ÿ���� ����ٴϴ� �޼���
    private void FollowToTarget()
    {
        if (target == null) return;

        Vector3 followPos = new Vector3(target.position.x, yOffset, target.position.z + zOffset);
        transform.position = followPos;
    }

    //Ÿ���� �ٶ󺸴� �޼���
    private void RotateToTarget()
    {
        if (target == null) return;

        transform.LookAt(target);
    }
}
