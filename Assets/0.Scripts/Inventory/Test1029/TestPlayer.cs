using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("�κ��丮")]
    public TestInventory inventory;

    void Update()
    {
        // ���콺 ��Ŭ�� (��ȣ�ۿ�)
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Click");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit3D;
            if (Physics.Raycast(ray, out hit3D))
                HitCheckObject(hit3D);
        }

        // ���� 1 (������)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // �κ��丮 1��° ĭ ������ ���� �ҷ���
            ItemModel item = inventory.GetItem(0);
            Debug.Log($"Using item: {(item != null ? item.itemName : "None")}");
            if (item != null && item.useAction != null)
            {
                item.useAction.Use(item, gameObject);
                // ������ ��� �� �κ��丮���� ����
                inventory.RemoveItem(0);
            }
        }
    }

    void HitCheckObject(RaycastHit hit3D)
    {
        IObjectItem clickInterface = hit3D.transform.gameObject.GetComponent<IObjectItem>();

        if (clickInterface != null)
        {
            ItemModel item = clickInterface.ClickItem();
            Debug.Log($"{item.itemName}");
            inventory.AddItem(item);
            // ���⼭ ������ ȹ�� ȿ���� ��� ����
            // ��: AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // ������ ������Ʈ ����
            //Destroy(hit3D.transform.gameObject);
            hit3D.transform.gameObject.SetActive(false);
        }
    }
}