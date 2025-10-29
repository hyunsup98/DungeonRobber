using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    // ���� ���� ������ ����Ʈ�� �ҷ��ͼ� UI�� ǥ��
    // EŰ�� ������ �κ��丮 â�� ������ ������ ��� < Item/Player.cs���� ó�� >

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
            // ���� ���� ������ ���� ����
            //for (int i = 0; i < itemSlots.Length; i++)
            //{
            //    //Debug.Log($"Checking slot {i + 1}: IsIn={itemSlots[i].IsIn}, ItemName={(itemSlots[i].Item != null ? itemSlots[i].Item.Name : "null")}");
            //    if (itemSlots[i].IsIn && itemSlots[i].Item.Name == newItem.Name)
            //    {
            //        itemSlots[i].AddCount(1);
            //        itemSlots[i].UpdateCountText();
            //        Debug.Log($"������ '{newItem.Name}'(��)�� �κ��丮 {i + 1}��°�� �߰��Ǿ����ϴ�. ���� ����: {itemSlots[i].Item.Count}");
            //        return;
            //    }
            //}

            // �� ���Կ� ������ �߰�
            for (int i = 0; i < itemSlots.Length; i++)
            {
                Debug.Log(itemSlots[i]);
                if (!itemSlots[i].IsIn)
                {
                    Item instNewItem = Instantiate(newItem);
                    itemSlots[i].SetSlot(instNewItem, true);
                    Debug.Log($"������ '{instNewItem.Name}'(��)�� �κ��丮 {i + 1}��°�� �߰��Ǿ����ϴ�.");
                    return;
                }
            }

            // �κ��丮 ������ ���� á�� ��
            Debug.Log("�κ��丮 ������ ���� á���ϴ�. �������� �߰��� �� �����ϴ�.");
        }



        if (HasItem(newItem.Name))
        {
            Debug.Log($"�̹� '{newItem.Name}' �������� ���� ���Դϴ�.");
            return;
        }
        // �̹� �������� ��� ���� ����????

        for (int i = 0; i < invenArray.Length; i++)
        {
            if (invenArray[i] == null)
            {
                invenArray[i] = newItem;
                // ������ ������ ������Ʈ�� �κ��丮�� ����
                newItem.gameObject.SetActive(false);


                Debug.Log($"������ '{newItem.Name}'(��)�� �κ��丮�� �߰��Ǿ����ϴ�.");
                // �����Կ� �ڵ� �Ҵ��ϴ� ��� �߰� ����
                return;
            }
        }
        Debug.Log("�κ��丮�� ���� á���ϴ�. �������� �߰��� �� �����ϴ�.");
    }

    //void Awake()
    //{
    //    //player = GameObject.FindGameObjectWithTag("Player");
    //    //invenArray = new Item[invenCapacity];


    //}

    SellableItem necklace;

    private void Start()
    {
        // ����: 10ĭ¥�� ���� �迭 �Ҵ�
        //itemSlots = new Slot[invenCapacity];

        
    }

    //void Update()
    //{
    //    TextMeshProUGUI invenText = GameObject.FindGameObjectWithTag("InvenText")?.GetComponent<TextMeshProUGUI>();
    //    invenText.text = $"Inventory: {String.Join(" / ", Array.ConvertAll(invenArray, item => item != null ? item.Name : " "))}";
    //}

    /// <summary>
    /// ������ ���� Ȯ��
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns>�������� bool</returns>
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



    ///////////////////////////////////////////////// �׽�Ʈ�� �޼��� /////////////////////////////////////////////////

    private WeaponItem defaultWeapon;
    private ConsumableItem speedRune;

    public Item GetRandomItem()
    {
        speedRune = gameObject.AddComponent<ConsumableItem>();
        speedRune.Name = "Speed Rune";
        speedRune.Type = ItemType.Consumable;
        speedRune.Grade = ItemGrade.Rare;
        speedRune.Description = "����ϸ� ���� �ð� ���� �̵� �ӵ��� �������� ��. ���� Ž���̳� ���� ���� ���� �����ϴ�.";
        speedRune.ConsumeType = ConsumableType.Effect;
        speedRune.Power = 2f;
        speedRune.Duration = 5;


        Item[] randomItems = new Item[] { speedRune, necklace };

        return randomItems[UnityEngine.Random.Range(0, randomItems.Length)];
    }
}
