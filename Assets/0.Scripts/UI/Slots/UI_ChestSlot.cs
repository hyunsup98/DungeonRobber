using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChestSlot : MonoBehaviour
{
    public Item item;
    [SerializeField] private Image itemImg;                 //아이템의 이미지를 보여줄 이미지
    [SerializeField] private float loadingTime;             //로딩바가 돌아갈 시간
    [SerializeField] private AudioClip clip;                //아이템이 나올 때 재생할 클립
    [SerializeField] private GameObject obj_DuringLoad;     //아이템이 있을 때 확인할 때 까지 가려주는 오브젝트
    [SerializeField] private RectTransform loadingImg;      //로딩바 이미지의 RectTransform
    [SerializeField] private float rotateSpeed = 200f;      //로딩바가 돌아갈 속도



    public void SlotInit()
    {
        if (item != null)
        {
            if (obj_DuringLoad != null && !obj_DuringLoad.activeSelf)
            {
                obj_DuringLoad.SetActive(true);
            }

            if(loadingImg != null)
            {
                loadingImg.rotation = Quaternion.identity;
                loadingImg.gameObject.SetActive(false);
            }
        }
        else
        {
            obj_DuringLoad.SetActive(false);
        }

        itemImg.sprite = null;
        Color color = itemImg.color;
        color.a = 0;
        itemImg.color = color;
    }

    /// <summary>
    /// 슬롯의 아이템을 공개하는 메서드
    /// </summary>
    /// <param name="isFirst"> 이 슬롯을 처음으로 여는건지 </param>
    /// <returns></returns>
    public IEnumerator CoOpenSlot(bool isOpened)
    {
        if(!isOpened)
        {
            var timer = 0f;
            loadingImg.gameObject.SetActive(true);

            while (timer <= loadingTime)
            {
                timer += Time.deltaTime;
                loadingImg.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
                yield return null;
            }
        }

        obj_DuringLoad.SetActive(false);
        itemImg.sprite = item.itemImage;
        Color color = itemImg.color;
        color.a = 255f;
        itemImg.color = color;
        SoundManager.Instance.PlaySoundEffect(clip);
    }
}
