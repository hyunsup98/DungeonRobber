﻿using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    float moveSpeedOriginal;
    private int gold = 0;

    public int Gold { get => gold; set => gold = value; }

    GameObject inventory;

    void Awake()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory");
        inventory.SetActive(false);
    }

    void Update()
    {
        // wasd move
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        transform.Translate(new Vector3(moveX, 0, moveZ));

        //Debug.Log("Player Position: " + transform.position);

        // 아이템 사용: 숫자 1~9키

        //TODO: 옵저버 패턴 추가
        TextMeshProUGUI goldText = GameObject.FindGameObjectWithTag("GoldText")?.GetComponent<TextMeshProUGUI>();
        if (goldText != null)
        {
            goldText.text = $"Gold: {Gold}";
        }

        if (Input.anyKeyDown == false)
        {
            return;
        }

        ItemTest itemTest = gameObject.GetComponent<ItemTest>();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            itemTest.UseItem("speedRune");
        }
        // 임시로 2번 키를 눌러 판매한다고 가정
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemTest.SellItem();
        }

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
            inventory.SetActive(!inventory.activeSelf);
        }
    }

    // 임시로 인벤토리에 아이템 추가하는 메서드
    internal void AddItemToInventory()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Inventory inven = inventory.GetComponent<Inventory>();

            if (inven != null)
            {
                Item item = inven.GetRandomItem();
                inven.AddItem(item);
            }
        }
        
    }
}