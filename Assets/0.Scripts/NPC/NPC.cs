using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    protected bool isCanInteractive;            //true면 상호작용이 가능한 상태라 상호작용 메서드 호출 가능

    protected abstract void DoInteractive();    //상호작용 키를 눌렀을 때 할 메서드

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            UIManager.Instance.OnOffUI(UIManager.Instance.textInteractive.gameObject, true);
            isCanInteractive = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            UIManager.Instance.OnOffUI(UIManager.Instance.textInteractive.gameObject, false);
            isCanInteractive = false;
        }
    }
}
