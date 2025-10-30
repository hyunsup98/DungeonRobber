using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    // 보유 중인 아이템 리스트를 불러와서 UI로 표시
    // E키를 누르면 인벤토리 창이 열리고 닫히는 기능 < Item/Player.cs에서 처리 >

    [SerializeField] int invenCapacity = 10;

    private GameObject player;
    private Item[] invenArray;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        invenArray = new Item[invenCapacity];
    }

    void Update()
    {
        TextMeshProUGUI invenText = GameObject.FindGameObjectWithTag("InvenText")?.GetComponent<TextMeshProUGUI>();
        invenText.text = $"Inventory: {String.Join(" / ", Array.ConvertAll(invenArray, item => item != null ? item.Name : " "))}";
    }

    /// <summary>
    /// 아이템 보유 확인
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns>보유여부 bool</returns>
    public bool HasItem(string itemName)
    {
        foreach (var invItem in invenArray)
        {
            if (invItem != null && invItem.Name == itemName)
            {
                return true;
            }
        }
        return false;
    }

    public void AddItem(Item newItem)
    {
        if (HasItem(newItem.Name))
        {
            Debug.Log($"이미 '{newItem.Name}' 아이템을 보유 중입니다.");
            return;
        }
        // 이미 보유중인 경우 수량 증가????

        for (int i = 0; i < invenArray.Length; i++)
        {
            if (invenArray[i] == null)
            {
                invenArray[i] = newItem;
                Debug.Log($"아이템 '{newItem.Name}'(이)가 인벤토리에 추가되었습니다.");
                // 퀵슬롯에 자동 할당하는 기능 추가 예정
                return;
            }
        }
        Debug.Log("인벤토리가 가득 찼습니다. 아이템을 추가할 수 없습니다.");
    }

    ///////////////////////////////////////////////// 테스트용 메서드 /////////////////////////////////////////////////

    private WeaponItem defaultWeapon;
    private ConsumableItem speedRune;
    private SellableItem necklace;

    public Item GetRandomItem()
    {
        speedRune = gameObject.AddComponent<ConsumableItem>();
        speedRune.Name = "Speed Rune";
        speedRune.Type = ItemType.Consumable;
        speedRune.Grade = ItemGrade.Rare;
        speedRune.Description = "사용하면 일정 시간 동안 이동 속도가 빨라지는 룬. 던전 탐험이나 적을 피할 때에 유용하다.";
        speedRune.ConsumeType = ConsumableType.Effect;
        speedRune.Power = 2f;
        speedRune.Duration = 5;

        necklace = gameObject.AddComponent<SellableItem>();
        necklace.Name = "Cursed Skul";
        necklace.Type = ItemType.Sellable;
        necklace.Grade = ItemGrade.Unique;
        necklace.Description = "으스스한 기운을 풍기는 저주받은 해골. 상당한 가치가 있어 보인다.";
        necklace.Price = 150;

        Item[] randomItems = new Item[] { speedRune, necklace };

        return randomItems[UnityEngine.Random.Range(0, randomItems.Length)];
    }
}
