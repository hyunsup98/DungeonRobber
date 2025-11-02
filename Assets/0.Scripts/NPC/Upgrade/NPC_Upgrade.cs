using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Upgrade : NPC
{
    private void Update()
    {
        if(isCanInteractive)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                DoInteractive();
            }
        }
    }

    protected override void DoInteractive()
    {

    }
}
