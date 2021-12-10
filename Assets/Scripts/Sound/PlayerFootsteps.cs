using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.gameObject.activeSelf && other.tag == "Untagged")) return;

        PlayerSoundManager.currentManager.PlayFootStep();

    }
}
