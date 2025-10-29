using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : MonoBehaviour
{
    public List<ItemModel> items;

    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private Slot2[] slots;

#if UNITY_EDITOR
    private void OnValidate()
    {
        slots = slotParent.GetComponentsInChildren<Slot2>();
    }
#endif

    void Awake()
    {
        FreshSlot();
    }

    public void FreshSlot()
    {
        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            slots[i].item = items[i];
        }
        for (; i < slots.Length; i++)
        {
            slots[i].item = null;
        }
    }

    public void AddItem(ItemModel _item)
    {
        if (items.Count < slots.Length)
        {
            items.Add(_item);
            FreshSlot();
        }
        else
        {
            Debug.Log("½½·ÔÀÌ °¡µæ Â÷ ÀÖ½À´Ï´Ù.");
        }
    }
}
