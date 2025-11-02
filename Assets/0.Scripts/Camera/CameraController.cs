using UnityEngine;

public class CameraController : MonoBehaviour
{
    //바라볼 타겟
    [SerializeField] private Transform target;

    //쉐이더로 칠해줄 쿼드 트랜스폼
    [SerializeField] private Transform quadTrans;

    //카메라가 떠 있을 y좌표 값
    [SerializeField] private float yOffset = 20f;

    //쿼터뷰를 위해 플레이어 위치에서 z좌표가 얼마나 아래에 있을지에 대한 값
    [SerializeField] private float zOffset = -10f;

    private void Awake()
    {
        if(Player_Controller.Instance != null && quadTrans != null)
        {
            Player_Controller.Instance.fieldOfView.SetFovQuad(quadTrans);
            target = Player_Controller.Instance.transform;
        }
    }

    private void LateUpdate()
    {
        FollowToTarget();
        RotateToTarget();
    }

    //타겟을 따라다니는 메서드
    private void FollowToTarget()
    {
        if (target == null) return;

        Vector3 followPos = new Vector3(target.position.x, yOffset, target.position.z + zOffset);
        transform.position = followPos;
    }

    //타겟을 바라보는 메서드
    private void RotateToTarget()
    {
        if (target == null) return;

        transform.LookAt(target);
    }
}
