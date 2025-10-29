using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    // 보유 중인 아이템 리스트를 불러와서 UI로 표시
    // E키를 누르면 인벤토리 창이 열리고 닫히는 기능 < Item/Player.cs에서 처리 >

    [SerializeField] int invenCapacity = 1;

    private GameObject player;
    private Item[] invenArray;

    public Slot[] itemSlots;
    public static InventoryController instance;

    public void AddItem(Item newItem)
    {
        Debug.Log($"AddItem Called: {newItem.Name}");

        if (itemSlots != null)
        {
            // 보유 중인 아이템 수량 증가
            //for (int i = 0; i < itemSlots.Length; i++)
            //{
            //    //Debug.Log($"Checking slot {i + 1}: IsIn={itemSlots[i].IsIn}, ItemName={(itemSlots[i].Item != null ? itemSlots[i].Item.Name : "null")}");
            //    if (itemSlots[i].IsIn && itemSlots[i].Item.Name == newItem.Name)
            //    {
            //        itemSlots[i].AddCount(1);
            //        itemSlots[i].UpdateCountText();
            //        Debug.Log($"아이템 '{newItem.Name}'(이)가 인벤토리 {i + 1}번째에 추가되었습니다. 현재 수량: {itemSlots[i].Item.Count}");
            //        return;
            //    }
            //}

            // 빈 슬롯에 아이템 추가
            for (int i = 0; i < itemSlots.Length; i++)
            {
                Debug.Log(itemSlots[i]);
                if (!itemSlots[i].IsIn)
                {
                    Item instNewItem = Instantiate(newItem);
                    itemSlots[i].SetSlot(instNewItem, true);
                    Debug.Log($"아이템 '{instNewItem.Name}'(이)가 인벤토리 {i + 1}번째에 추가되었습니다.");
                    return;
                }
            }

            // 인벤토리 슬롯이 가득 찼을 때
            Debug.Log("인벤토리 슬롯이 가득 찼습니다. 아이템을 추가할 수 없습니다.");
        }



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
                // 실제로 아이템 오브젝트를 인벤토리에 보관
                newItem.gameObject.SetActive(false);


                Debug.Log($"아이템 '{newItem.Name}'(이)가 인벤토리에 추가되었습니다.");
                // 퀵슬롯에 자동 할당하는 기능 추가 예정
                return;
            }
        }
        Debug.Log("인벤토리가 가득 찼습니다. 아이템을 추가할 수 없습니다.");
    }

    //void Awake()
    //{
    //    //player = GameObject.FindGameObjectWithTag("Player");
    //    //invenArray = new Item[invenCapacity];


    //}

    SellableItem necklace;

    private void Start()
    {
        // 예시: 10칸짜리 슬롯 배열 할당
        //itemSlots = new Slot[invenCapacity];

        
    }

    //void Update()
    //{
    //    TextMeshProUGUI invenText = GameObject.FindGameObjectWithTag("InvenText")?.GetComponent<TextMeshProUGUI>();
    //    invenText.text = $"Inventory: {String.Join(" / ", Array.ConvertAll(invenArray, item => item != null ? item.Name : " "))}";
    //}

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



    ///////////////////////////////////////////////// 테스트용 메서드 /////////////////////////////////////////////////

    private WeaponItem defaultWeapon;
    private ConsumableItem speedRune;

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


        Item[] randomItems = new Item[] { speedRune, necklace };

        return randomItems[UnityEngine.Random.Range(0, randomItems.Length)];
    }
}
