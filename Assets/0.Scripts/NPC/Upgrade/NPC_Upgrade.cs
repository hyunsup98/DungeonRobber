using UnityEngine;

public class NPC_Upgrade : NPC
{
    [SerializeField] private GameObject upgradeUI;

    private void Update()
    {
        if(isCanInteractive)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                UIManager.Instance.OnOffUI(UIManager.Instance.textInteractive.gameObject, false);
                DoInteractive();
            }
        }
    }

    protected override void DoInteractive()
    {
        if(upgradeUI != null)
        {
            upgradeUI.SetActive(true);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.CompareTag("Player"))
        {
            upgradeUI.SetActive(false);
        }
    }
}
