using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Consumable,
    Sellable,
    Weapon
}

public enum ItemGrade
{
    Normal,
    Rare,
    Unique,
    Epic
}

public enum ConsumableType
{
    Attack, Shield, Effect
}

public interface ISellable
{
    int Price { get; }
    void Sell();
}

public interface IConsumable
{
    ConsumableType ConsumeType { get; }
    float Power { get; }
    int Duration { get; }
    void Use();
}

public class Item : MonoBehaviour
{
    string _name;
    ItemType _type;
    ItemGrade _grade;
    string _description;
    int _count;
    Sprite _image;

    public string Name { get => _name; set => _name = value; }
    public ItemType Type { get => _type; set => _type = value; }
    public ItemGrade Grade { get => _grade; set => _grade = value; }
    public string Description { get => _description; set => _description = value; }
    public int Count { get => _count; set => _count = value; }
    public Sprite IconImage { get => _image; set => _image = value; }
}

public class ConsumableItem : Item, IConsumable
{
    ConsumableType _consumeType;
    float _power;
    int _duration;
    public ConsumableType ConsumeType { get => _consumeType; set => _consumeType = value; }
    public float Power { get => _power; set => _power = value; }
    public int Duration { get => _duration; set => _duration = value; }
    public void Use()
    {
        Debug.Log($"'{Name}' ���!");

        if (ConsumeType == ConsumableType.Effect)
        {
            // ���� �̸��� "�ӵ� ��"�̶�� �̵� �ӵ� ���� ȿ�� ����
            if(Name == "�ӵ� ��")
            {
                Player player = FindObjectOfType<Player>();
                if(player != null)
                {
                    player.ApplySpeedEffect(Power, Duration);
                }
            }
        }
    }
}

public class SellableItem : Item, ISellable
{
    int _price;
    public int Price { get => _price; set => _price = value; }
    public void Sell()
    {
        Debug.Log($"'{Name}'��(��) �Ǹ��߽��ϴ�.");

        // �÷��̾� ��带 price��ŭ ����
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.AddGold(Price);
        }
    }
}

public class WeaponItem : Item
{
    float _attackPower;
    public float AttackPower { get => _attackPower; set => _attackPower = value; }
}