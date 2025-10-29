using UnityEngine;

[CreateAssetMenu(menuName = "Items/Actions/Heal")]
public class HealAction : ItemAction
{
    public int healAmount = 20;

    public override void Use(ItemModel item, GameObject user)
    {
        // user에서 플레이어 컴포넌트를 찾아 처리
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

        // 예: 사운드, 파티클 등도 여기서 재생
        // AudioSource.PlayClipAtPoint(...);
    }
}