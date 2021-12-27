using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using static UEventHandler;

public class PlayerCutsceneManager : MonoBehaviour
{
    public static PlayerCutsceneManager currentPlayerCutsceneManager;
    public static UEvent OnIntroFinished = new UEvent();
    public static UEvent OnIntroStarted = new UEvent();
    public static UEvent OnEndingStarted = new UEvent();
    public static UEvent OnEndingFadeIn = new UEvent();

    public static UEvent OnCreditsStarted = new UEvent();
    public static bool isIntroEnabled;
    public static bool isInCutscene;


    PlayableDirector director;
    public bool playIntro = true;

    public PlayableAsset introClip;

    public UEventHandler eventHandler = new UEventHandler();
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
