using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    // ���� ���� ������ ����Ʈ�� �ҷ��ͼ� UI�� ǥ��
    // EŰ�� ������ �κ��丮 â�� ������ ������ ��� < Item/Player.cs���� ó�� >

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

    public void AddItem(Item newItem)
    {
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
                Debug.Log($"������ '{newItem.Name}'(��)�� �κ��丮�� �߰��Ǿ����ϴ�.");
                // �����Կ� �ڵ� �Ҵ��ϴ� ��� �߰� ����
                return;
            }
        }
        Debug.Log("�κ��丮�� ���� á���ϴ�. �������� �߰��� �� �����ϴ�.");
    }

    ///////////////////////////////////////////////// �׽�Ʈ�� �޼��� /////////////////////////////////////////////////

    private WeaponItem defaultWeapon;
    private ConsumableItem speedRune;
    private SellableItem necklace;

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

        necklace = gameObject.AddComponent<SellableItem>();
        necklace.Name = "Cursed Skul";
        necklace.Type = ItemType.Sellable;
        necklace.Grade = ItemGrade.Unique;
        necklace.Description = "�������� ����� ǳ��� ���ֹ��� �ذ�. ����� ��ġ�� �־� ���δ�.";
        necklace.Price = 150;

        Item[] randomItems = new Item[] { speedRune, necklace };

        return randomItems[UnityEngine.Random.Range(0, randomItems.Length)];
    }
}
