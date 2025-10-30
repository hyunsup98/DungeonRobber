using UnityEngine;

[CreateAssetMenu(menuName = "Items/Actions/Speed")]
public class SpeedAction : ItemAction
{
    public int speedAmount = 10;

    public override void Use(Item item, GameObject user)
    {
        // user���� �÷��̾� ������Ʈ�� ã�� ó��
        var player = user.GetComponent<Player_Controller>();
        if (player != null)
        {
            // �̵� �ӵ� ���� �Լ� ȣ��
            Debug.Log("�̵� �ӵ� ������");
            //player.moveSpeed += speedAmount;
        }
        else
        {
            Debug.LogWarning("SpeedAction: user has no Player component.");
        }

        // ����, ��ƼŬ � ���⼭ ���
        // AudioSource.PlayClipAtPoint(...);
    }
}