using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UEventHandler;

public class PlayerDetection : MonoBehaviour
{

    public delegate void PlayerNearby(bool isNear);

    //public event PlayerNearby OnPlayerNearby;
    public UEvent<bool> OnPlayerNearby= new UEvent<bool>();

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
