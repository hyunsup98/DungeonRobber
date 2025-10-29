using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("인벤토리")]
    public TestInventory inventory;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null)
            {
                HitCheckObject(hit);
            }
        }
    }

    void HitCheckObject(RaycastHit2D hit)
    {
        IObjectItem clickInterface = hit.transform.gameObject.GetComponent<IObjectItem>();

        if (clickInterface != null)
        {
            ItemModel item = clickInterface.ClickItem();
            Debug.Log($"{item.itemName}");
            inventory.AddItem(item);
        }
    }
}