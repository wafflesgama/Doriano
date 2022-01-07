using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uevents;

public class PlayerDetection : MonoBehaviour
{

    public delegate void PlayerNearby(bool isNear);

    //public event PlayerNearby OnPlayerNearby;
    public Uevent<bool> OnPlayerNearby= new Uevent<bool>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            OnPlayerNearby.TryInvoke(true);
        //OnPlayerNearby?.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            OnPlayerNearby.TryInvoke(false);
            //OnPlayerNearby?.Invoke(false);
    }
}
