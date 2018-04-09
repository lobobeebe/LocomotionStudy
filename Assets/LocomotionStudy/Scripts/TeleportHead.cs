using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportHead : MonoBehaviour
{
    public OverHeadTeleport TeleportScript;

    public void OnTriggerEnter(Collider other)
    {
        TeleportScript.OnTeleport();
    }
}
