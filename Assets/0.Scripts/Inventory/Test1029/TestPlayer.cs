using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("�κ��丮")]
    public TestInventory inventory;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Click");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit3D;
            if (Physics.Raycast(ray, out hit3D))
                HitCheckObject(hit3D);
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
            Destroy(hit3D.transform.gameObject);
        }
    }
}