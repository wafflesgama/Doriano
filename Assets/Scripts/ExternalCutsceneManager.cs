using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class ExternalCutsceneManager : MonoBehaviour
{
    public PlayableDirector endingDirector;
    UEventHandler eventHandler = new UEventHandler();
    void Start()
    {
        PlayerCutsceneManager.OnEndingStarted.Subscribe(eventHandler, StartEnding);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public async void StartEnding()
    {
        await Task.Delay(1000);
        endingDirector.Play();
    }

    public void FadeInEnding()
    {
        PlayerCutsceneManager.OnEndingFadeIn.TryInvoke();

    }
    public void StartCredits()
    {
        PlayerCutsceneManager.OnCreditsStarted.TryInvoke();
    }
}
