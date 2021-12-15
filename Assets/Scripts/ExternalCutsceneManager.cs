using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalCutsceneManager : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCredits()
    {
        PlayerCutsceneManager.OnCreditsStarted.TryInvoke();
    }
}
