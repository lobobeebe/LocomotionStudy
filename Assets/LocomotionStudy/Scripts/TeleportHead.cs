using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportHead : MonoBehaviour
{
    public OverHeadTeleport TeleportScript;

    private bool mCanTeleport = true;

    public void OnTriggerEnter(Collider other)
    {
        if (mCanTeleport)
        {
            TeleportScript.OnTeleport();
            //mCanTeleport = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        mCanTeleport = true;
    }
}
