using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("�κ��丮")]
    public Inventory inventory;

    [Header("������")]
    public QuickSlot_Controller quickSlots;

    void Update()
    {
        // ���콺 ��Ŭ�� (��ȣ�ۿ�)
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Click");
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit3D;
            //if (Physics.Raycast(ray, out hit3D))
            //    HitCheckObject(hit3D);
        }

        if (Input.anyKeyDown)
        {
            // ���� 1~6�� (������)
            //for (int i = 0; i < 6; i++)
            //{
            //    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            //    {
            //        // ������ i��° ĭ ������ ���� �ҷ���
            //        Item item = quickSlots.GetItem(i);
            //        Debug.Log($"Using item: {(item != null ? item.itemName : "None")}");
            //        if (item != null && item.useAction != null)
            //        {
            //            item.useAction.Use(item, gameObject);
            //            // ������ ��� �� ������, �κ��丮���� ����
            //            quickSlots.RemoveItem(i);
            //        }
            //    }
            //}

            // Tab Ű (�κ��丮 ����/�ݱ�)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (inventory != null)
                    inventory.ToggleInventory();
            }
        }
    }

    //void HitCheckObject(RaycastHit hit3D)
    //{
    //    IObjectItem clickInterface = hit3D.transform.gameObject.GetComponent<IObjectItem>();

    //    if (clickInterface != null)
    //    {
    //        Item item = clickInterface.ClickItem();
    //        bool isAdded = inventory.AddItem(item);
            
    //        // ������ ȹ�� ȿ����
    //        // AudioSource.PlayClipAtPoint(pickupSound, transform.position);

    //        // ������ ������Ʈ ����
    //        // Destroy ���� �ʰ� ��Ȱ��ȭ ���� ���� ��
    //        //Destroy(hit3D.transform.gameObject);
    //        if(isAdded) hit3D.transform.gameObject.SetActive(false);
    //    }
    //}
}