using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    //바라볼 타겟
    [SerializeField] private Transform target;

    //카메라가 떠 있을 y좌표 값
    [SerializeField] private float yOffset = 20f;

    //쿼터뷰를 위해 플레이어 위치에서 z좌표가 얼마나 아래에 있을지에 대한 값
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

    /// <summary>
    /// 게임 화면의 마우스 포인터 좌표를 월드좌표로 변환 후 반환하는 메서드
    /// </summary>
    /// <returns> 마우스 좌표의 월드좌표 벡터 </returns>
    public Vector3 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = transform.position.y;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
