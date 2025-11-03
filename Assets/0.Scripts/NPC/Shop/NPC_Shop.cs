using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Shop : NPC
{
    [SerializeField] private GameObject shopUI;

    private void Update()
    {
        if (isCanInteractive)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                UIManager.Instance.OnOffUI(UIManager.Instance.textInteractive.gameObject, false);
                DoInteractive();
            }
        }
    }

    protected override void DoInteractive()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(true);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if(other.CompareTag("Player") && shopUI != null)
        {
            shopUI.SetActive(false);
        }
    }
}
