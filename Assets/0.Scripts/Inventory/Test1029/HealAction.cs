using UnityEngine;

[CreateAssetMenu(menuName = "Items/Actions/Heal")]
public class HealAction : ItemAction
{
    public int healAmount = 20;

    public override void Use(ItemModel item, GameObject user)
    {
        // user���� �÷��̾� ������Ʈ�� ã�� ó��
        var player = user.GetComponent<Player_Controller>();
        if (player != null)
        {
            player.CurrentHP += healAmount;
            Debug.Log($"Used {item.itemName}: healed {healAmount}");
        }
        else
        {
            Debug.LogWarning("HealAction: user has no Player component.");
        }

        // ��: ����, ��ƼŬ � ���⼭ ���
        // AudioSource.PlayClipAtPoint(...);
    }
}