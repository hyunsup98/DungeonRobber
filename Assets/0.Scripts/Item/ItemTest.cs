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
        // �⺻ ����: ���� �ܰ� (���� ������� ����)
        //defaultWeapon = gameObject.AddComponent<WeaponItem>();
        //defaultWeapon.Name = "���� �ܰ�";
        //defaultWeapon.Type = ItemType.Weapon;
        //defaultWeapon.Grade = ItemGrade.Normal;
        //defaultWeapon.Description = "�쿬�� ���� ��ó���� �ֿ� ���� �ܰ�. ���п� ������ �� �ּ� ������ �޼��� �� �־���. ���� ���� �ɱ�?";

        //// ������ 1��: �̵� �ӵ��� �������� ���� (���� �ð� 5��)
        //speedRune = gameObject.AddComponent<ConsumableItem>();
        //speedRune.Name = "�ӵ� ��";
        //speedRune.Type = ItemType.Consumable;
        //speedRune.Grade = ItemGrade.Rare;
        //speedRune.Description = "����ϸ� ���� �ð� ���� �̵� �ӵ��� �������� ��. ���� Ž���̳� ���� ���� ���� �����ϴ�.";
        //speedRune.ConsumeType = ConsumableType.Effect;
        //speedRune.Power = 2f;
        //speedRune.Duration = 5;

        //// ������ 2��: �Ǹ� ������ ������
        //necklace = gameObject.AddComponent<SellableItem>();
        //necklace.Name = "���ֹ��� �ذ�";
        //necklace.Type = ItemType.Sellable;
        //necklace.Grade = ItemGrade.Unique;
        //necklace.Description = "�������� ����� ǳ��� ���ֹ��� �ذ�. ����� ��ġ�� �־� ���δ�.";
        //necklace.Price = 150;
    }

    private void Start()
    {
        //Debug.Log($"�⺻ ����: {defaultWeapon.Name}({defaultWeapon.Grade})");
        //Debug.Log($"������ 1�� ������: {speedRune.Name}({speedRune.Grade})");
        //Debug.Log($"������ 2�� ������: {necklace.Name}({necklace.Grade})");
    }

    // ������ ��� �޼ҵ�
    public void UseItem(string itemName)
    {
        //if (itemName == "speedRune")
        //{
        //    speedRune.Use();
        //}
    }

    // ������ �Ǹ� �޼ҵ�
    public void SellItem()
    {
        //necklace.Sell();
    }
}
