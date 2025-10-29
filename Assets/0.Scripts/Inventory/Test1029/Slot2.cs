using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;

    private ItemModel _item;
    public ItemModel item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item != null)
            {
                image.sprite = _item.itemImage;
                image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                image.color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_item != null && ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.Show(_item);
            ItemTooltip.Instance.SetPosition(eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemTooltip.Instance != null)
            ItemTooltip.Instance.Hide();
    }
}