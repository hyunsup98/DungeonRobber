using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player;
    public Transform playerIcon;

    void LateUpdate()
    {
        if (player != null)
        {
            // 카메라 Y 고정, 플레이어 위치 따라가기
            Vector3 newPos = player.position;
            newPos.y = transform.position.y;
            transform.position = newPos;

            // 미니맵 아이콘 회전
            if (playerIcon != null)
            {
                Vector3 newPos1 = new Vector3(player.position.x, transform.position.y, player.position.z);
            }
        }
    }
}
