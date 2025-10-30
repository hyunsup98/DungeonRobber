using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("인벤토리")]
    public Inventory inventory;

    [Header("퀵슬롯")]
    public QuickSlots quickSlots;

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

        if (Input.anyKeyDown)
        {
            // 숫자 1~6번 (퀵슬롯)
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    // 퀵슬롯 i번째 칸 아이템 정보 불러옴
                    Item item = quickSlots.GetItem(i);
                    Debug.Log($"Using item: {(item != null ? item.itemName : "None")}");
                    if (item != null && item.useAction != null)
                    {
                        item.useAction.Use(item, gameObject);
                        // 아이템 사용 후 퀵슬롯, 인벤토리에서 제거
                        quickSlots.RemoveItem(i);
                    }
                }
            }

            // Tab 키 (인벤토리 열기/닫기)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (inventory != null)
                    inventory.ToggleInventory();
            }
        }
    }

    void HitCheckObject(RaycastHit hit3D)
    {
        IObjectItem clickInterface = hit3D.transform.gameObject.GetComponent<IObjectItem>();

        if (clickInterface != null)
        {
            Item item = clickInterface.ClickItem();
            bool isAdded = inventory.AddItem(item);
            
            // 아이템 획득 효과음
            // AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // 아이템 오브젝트 삭제
            // Destroy 하지 않고 비활성화 할지 고민 중
            //Destroy(hit3D.transform.gameObject);
            if(isAdded) hit3D.transform.gameObject.SetActive(false);
        }
    }
}