using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    float moveSpeedOriginal;
    private int gold = 0;

    public int Gold { get => gold; set => gold = value; }

    private GameObject InvenManager;
    private InventoryController inventory;
    private SellableItem necklace;

    void Awake()
    {
        InvenManager = GameObject.FindGameObjectWithTag("InvenManager");
        InvenManager.SetActive(false);

        
    }

    void Update()
    {
        //TODO: 옵저버 패턴 추가
        //TextMeshProUGUI goldText = GameObject.FindGameObjectWithTag("GoldText")?.GetComponent<TextMeshProUGUI>();
        //if (goldText != null)
        //{
        //    goldText.text = $"Gold: {Gold}";
        //}

        if (Input.anyKeyDown == false)
        {
            return;
        }

        
        // 인벤토리 슬롯 4칸 = 퀵슬롯
        


        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    itemTest.UseItem("speedRune");
        //}
        //// 임시로 2번 키를 눌러 판매한다고 가정
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    itemTest.SellItem();
        //}

        ToggleInventory();
        AddItemToInventory();
    }























    // moveSpeed를 power 배로 증가시키고 duration 초 후에 원래 속도로 되돌리는 메소드
    internal void ApplySpeedEffect(float power, int duration)
    {
        // 이동 속도 증가
        moveSpeedOriginal = moveSpeed;
        moveSpeed *= power;
        Debug.Log($"{duration}초간 속도 {power}배 증가!!!");
        // duration 초 후에 원래 속도로 되돌리기
        StartCoroutine(ResetSpeedAfterDuration(duration));
    }

    private IEnumerator ResetSpeedAfterDuration(int duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed = moveSpeedOriginal;
        Debug.Log($"속도 리셋");
    }

    internal void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"골드 {amount} 획득! 현재 골드: {Gold}");
    }

    void ToggleInventory()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InvenManager.SetActive(!InvenManager.activeSelf);
        }
    }

    // 임시로 인벤토리에 아이템 추가하는 메서드
    internal void AddItemToInventory()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("스페이스바 감지");

            necklace = gameObject.AddComponent<SellableItem>();
            necklace.Name = "저주받은 해골";
            necklace.Type = ItemType.Sellable;
            necklace.Grade = ItemGrade.Unique;
            necklace.Description = "으스스한 기운을 풍기는 저주받은 해골. 상당한 가치가 있어 보인다.";
            necklace.Price = 150;
            necklace.IconImage = Resources.Load("Assets/7.AssetStores/GUI_Parts/Icons/skill_icon_03.png") as Sprite;

            inventory = InvenManager.GetComponent<InventoryController>();
            inventory.AddItem(necklace);

            //inventory.GetRandomItem();
            //inventory.AddItem(item);
        }
        
    }
}