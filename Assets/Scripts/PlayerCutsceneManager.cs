using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using Uevents;

public class PlayerCutsceneManager : MonoBehaviour
{
    public static PlayerCutsceneManager currentPlayerCutsceneManager;
    public static Uevent OnIntroFinished = new Uevent();
    public static Uevent OnIntroStarted = new Uevent();
    public static Uevent OnEndingStarted = new Uevent();
    public static Uevent OnEndingFadeIn = new Uevent();

    public static Uevent OnCreditsStarted = new Uevent();
    public static bool isIntroEnabled;
    public static bool isInCutscene;


    PlayableDirector director;
    public bool playIntro = true;

    public PlayableAsset introClip;

    public UeventHandler eventHandler = new UeventHandler();
    private void Awake()
    {
        isIntroEnabled = playIntro;
        director = GetComponent<PlayableDirector>();
        if (playIntro) director.Play(introClip);
    }
    void Start()
    {
        currentPlayerCutsceneManager = this;
        GameManager.OnPlayerReset.Subscribe(eventHandler, HandlePlayerReset);
       OnIntroStarted.Subscribe(eventHandler, ()=> isInCutscene=true);
       OnIntroFinished.Subscribe(eventHandler, ()=> isInCutscene=false);
       OnEndingStarted.Subscribe(eventHandler, ()=> isInCutscene=true);
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }

    public void StartEnding()
    {
        OnEndingStarted.TryInvoke();
    }

    public void IntroStarted()
    {
        OnIntroStarted.TryInvoke();
    }
    public void IntroFinished()
    {
        OnIntroFinished.TryInvoke();
    }

    public void CreditsStarted()
    {
        OnCreditsStarted.TryInvoke();
    }

    private async void HandlePlayerReset()
    {
        await Task.Delay(GameManager.currentGameManager.resetFreezeDurationMs);
        //if (playIntro) director.Play(introClip);
    }
}
