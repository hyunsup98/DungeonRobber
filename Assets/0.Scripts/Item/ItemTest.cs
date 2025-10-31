using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour
{
    //private WeaponItem defaultWeapon;
    //private ConsumableItem speedRune;
    //private SellableItem necklace;

    private void Awake()
    {
        // 기본 무기: 낡은 단검 (아직 사용하지 않음)
        //defaultWeapon = gameObject.AddComponent<WeaponItem>();
        //defaultWeapon.Name = "낡은 단검";
        //defaultWeapon.Type = ItemType.Weapon;
        //defaultWeapon.Grade = ItemGrade.Normal;
        //defaultWeapon.Description = "우연히 던전 근처에서 주운 낡은 단검. 덕분에 던전에 들어갈 최소 조건을 달성할 수 있었다. 누가 쓰던 걸까?";

        //// 퀵슬롯 1번: 이동 속도가 빨라지는 물약 (지속 시간 5초)
        //speedRune = gameObject.AddComponent<ConsumableItem>();
        //speedRune.Name = "속도 룬";
        //speedRune.Type = ItemType.Consumable;
        //speedRune.Grade = ItemGrade.Rare;
        //speedRune.Description = "사용하면 일정 시간 동안 이동 속도가 빨라지는 룬. 던전 탐험이나 적을 피할 때에 유용하다.";
        //speedRune.ConsumeType = ConsumableType.Effect;
        //speedRune.Power = 2f;
        //speedRune.Duration = 5;

        //// 퀵슬롯 2번: 판매 가능한 아이템
        //necklace = gameObject.AddComponent<SellableItem>();
        //necklace.Name = "저주받은 해골";
        //necklace.Type = ItemType.Sellable;
        //necklace.Grade = ItemGrade.Unique;
        //necklace.Description = "으스스한 기운을 풍기는 저주받은 해골. 상당한 가치가 있어 보인다.";
        //necklace.Price = 150;
    }

    private void Start()
    {
        //Debug.Log($"기본 무기: {defaultWeapon.Name}({defaultWeapon.Grade})");
        //Debug.Log($"퀵슬롯 1번 아이템: {speedRune.Name}({speedRune.Grade})");
        //Debug.Log($"퀵슬롯 2번 아이템: {necklace.Name}({necklace.Grade})");
    }

    // 아이템 사용 메소드
    public void UseItem(string itemName)
    {
        //if (itemName == "speedRune")
        //{
        //    speedRune.Use();
        //}
    }

    // 아이템 판매 메소드
    public void SellItem()
    {
        //necklace.Sell();
    }
}
