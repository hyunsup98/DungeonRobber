using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChestData
{
    public Item item;
    public bool isOpened;
}

public class ChestSlots : MonoBehaviour
{
    [SerializeField] private Item test;

    public UI_ChestSlot[] slots = new UI_ChestSlot[8];
    public ChestData[] datas;

    private void Start()
    {
        datas = new ChestData[slots.Length];

        SetItems();
        OpenChest();
    }

    /// <summary>
    /// 처음 시작했을 때 랜덤으로 상자에 아이템을 세팅하는 메서드 
    /// </summary>
    private void SetItems()
    {
        var rand = Random.Range(1, 8);
        for(int i = 0; i < rand; i++)
        {
            //랜덤으로 아이템을 넣어서 datas에 넣어줌
            datas[i].item = Instantiate(test);
        }
    }

    public void OpenChest()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].item = datas[i].item;
            slots[i].SlotInit();
        }

        StartCoroutine(OpenSlotSequence());
    }

    private IEnumerator OpenSlotSequence()
    {
        for(int i = 0; i < slots.Length;i++)
        {
            if (slots[i].item == null)
                yield break;

            yield return StartCoroutine(slots[i].CoOpenSlot(datas[i].isOpened));
            if (!datas[i].isOpened)
            {
                datas[i].isOpened = true;
            }
        }
    }
}
