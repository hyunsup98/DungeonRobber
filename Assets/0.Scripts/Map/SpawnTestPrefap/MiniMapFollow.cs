using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player;
    public Transform playerIcon;

    void LateUpdate()
    {
        if (player != null)
        {
            // ī�޶� Y ����, �÷��̾� ��ġ ���󰡱�
            Vector3 newPos = player.position;
            newPos.y = transform.position.y;
            transform.position = newPos;

            // �̴ϸ� ������ ȸ��
            if (playerIcon != null)
            {
                Vector3 newPos1 = new Vector3(player.position.x, transform.position.y, player.position.z);
            }
        }
    }
}
