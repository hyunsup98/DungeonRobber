using UnityEngine;

[CreateAssetMenu(menuName = "Items/Actions/Speed")]
public class SpeedAction : ItemAction
{
    public int speedAmount = 10;

    public override void Use(Item item, GameObject user)
    {
        // user에서 플레이어 컴포넌트를 찾아 처리
        var player = user.GetComponent<Player_Controller>();
        if (player != null)
        {
            // 이동 속도 증가 함수 호출
            Debug.Log("이동 속도 증가됨");
            //player.moveSpeed += speedAmount;
        }
        else
        {
            Debug.LogWarning("SpeedAction: user has no Player component.");
        }

        // 사운드, 파티클 등도 여기서 재생
        // AudioSource.PlayClipAtPoint(...);
    }
}