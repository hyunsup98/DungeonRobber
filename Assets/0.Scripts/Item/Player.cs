using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    float moveSpeedOriginal;
    private int gold = 0;

    public int Gold { get => gold; set => gold = value; }

    void Update()
    {
        // wasd move
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        transform.Translate(new Vector3(moveX, 0, moveZ));

        //Debug.Log("Player Position: " + transform.position);

        // ������ ���: ���� 1~9Ű

        //TODO: ������ ���� �߰�
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
        // �ӽ÷� 2�� Ű�� ���� �Ǹ��Ѵٰ� ����
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemTest.SellItem();
        }
    }

    // moveSpeed�� power ��� ������Ű�� duration �� �Ŀ� ���� �ӵ��� �ǵ����� �޼ҵ�
    internal void ApplySpeedEffect(float power, int duration)
    {
        // �̵� �ӵ� ����
        moveSpeedOriginal = moveSpeed;
        moveSpeed *= power;
        Debug.Log($"{duration}�ʰ� �ӵ� {power}�� ����!!!");
        // duration �� �Ŀ� ���� �ӵ��� �ǵ�����
        StartCoroutine(ResetSpeedAfterDuration(duration));
    }

    private IEnumerator ResetSpeedAfterDuration(int duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed = moveSpeedOriginal;
        Debug.Log($"�ӵ� ����");
    }

    internal void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"��� {amount} ȹ��! ���� ���: {Gold}");
    }
}
