using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("인벤토리")]
    public TestInventory inventory;

    void Update()
    {
        // 마우스 우클릭 (상호작용)
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Click");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit3D;
            if (Physics.Raycast(ray, out hit3D))
                HitCheckObject(hit3D);
        }

        // 숫자 1 (퀵슬롯)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 인벤토리 1번째 칸 아이템 정보 불러옴
            ItemModel item = inventory.GetItem(0);
            Debug.Log($"Using item: {(item != null ? item.itemName : "None")}");
            if (item != null && item.useAction != null)
            {
                item.useAction.Use(item, gameObject);
                // 아이템 사용 후 인벤토리에서 제거
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
            // 여기서 아이템 획득 효과음 재생 가능
            // 예: AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // 아이템 오브젝트 삭제
            //Destroy(hit3D.transform.gameObject);
            hit3D.transform.gameObject.SetActive(false);
        }
    }
}