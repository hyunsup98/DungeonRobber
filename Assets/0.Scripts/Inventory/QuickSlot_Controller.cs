using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot_Controller : MonoBehaviour
{
    [Header("인벤토리")]
    public Inventory inventory;

    public List<Item> items;

    [SerializeField]
    private Transform slotParent;
    private QuickSlot[] slots;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (slotParent != null)
            slots = slotParent.GetComponentsInChildren<QuickSlot>();
    }
#endif

    void Awake()
    {
        items = new List<Item>() { null, null, null, null, null, null };
        FreshSlot();
    }

    public void FreshSlot()
    {
        if (slotParent == null)
            return;

        if (slots == null || slots.Length == 0)
            slots = slotParent.GetComponentsInChildren<QuickSlot>();

        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            // 퀵슬롯 소유자/인덱스 설정
            //slots[i].ownerQuickSlots = this;
            slots[i].Item = items[i];
        }
        for (; i < slots.Length; i++)
        {
            //slots[i].ownerQuickSlots = this;
            slots[i].Item = null;
        }
    }

    // 퀵슬롯은 6칸으로 정해져있다
    // 가장 처음 null인 칸에 아이템 추가
    public void AddItem(Item item)
    {
        // 빈 칸 찾기
        int idx = items.FindIndex(x => x == null);
        if (idx != -1)
        {
            items[idx] = item;
            FreshSlot();
        }
        else
        {
            Debug.Log("퀵슬롯이 가득 차 있습니다.");
        }
    }

    public Item GetItem(int idx)
    {
        return slots[idx].Item;
    }
    
    // 퀵슬롯 아이템 사용 후 제거
    internal void RemoveItem(int idx)
    {
        if (idx < 0 || idx >= items.Count) return;
        inventory?.RemoveItem(items[idx]);
        Debug.Log($"{items[idx]}이 제거되었습니다.");
        items[idx] = null;
        FreshSlot();
    }

    // 퀵슬롯 등록 해제
    internal void RemoveItem(Item item)
    {
        int idx = items.FindIndex(x => x == item);
        Debug.Log($"퀵슬롯에서 {item.name}이 제거되었습니다.");
        items[idx] = null;
        FreshSlot();
    }
}
