using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{

    public delegate void PlayerNearby(bool isNear);

    public event PlayerNearby OnPlayerNearby;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag== "Player")
            OnPlayerNearby?.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            OnPlayerNearby?.Invoke(false);
    }
}
