using UnityEngine;

// 간단한 플레이어 이동 스크립트
public class SimpleMoveTest : MonoBehaviour
{
    // 이동 속도
    public float moveSpeed = 5f;

    // Update은 매 프레임마다 호출됨
    void Update()
    {
        // 입력값 받기 (WASD 또는 방향키)
        float h = Input.GetAxis("Horizontal");  // A, D 또는 ←, →
        float v = Input.GetAxis("Vertical");    // W, S 또는 ↑, ↓

        // 이동 방향 벡터 계산
        Vector3 moveDir = new Vector3(h, 0, v);

        // 정규화해서 대각선 이동 속도 보정
        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        // Transform 기반 이동
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        // 이동 방향으로 회전 (이동 중일 때만)
        if (moveDir != Vector3.zero)
            transform.forward = moveDir;
    }
}
