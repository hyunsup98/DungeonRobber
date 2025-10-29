using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("인벤토리")]
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
            // 여기서 아이템 획득 효과음 재생 가능
            // 예: AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // 아이템 오브젝트 삭제
            Destroy(hit3D.transform.gameObject);
        }
    }
}