using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using static UEventHandler;

public class PlayerCutsceneManager : MonoBehaviour
{
    public static UEvent OnIntroFinished=new UEvent();
    //public static Action OnIntroFinished;
    public static UEvent OnIntroStarted= new UEvent();
    //public static Action OnIntroStarted;
    public static bool isIntroEnabled;


    PlayableDirector director;
    public bool playIntro = true;
    public UEventHandler eventHandler = new UEventHandler();
    private void Awake()
    {
        isIntroEnabled = playIntro;
        director = GetComponent<PlayableDirector>();
        if (playIntro) director.Play();
    }
    void Start()
    {
        GameManager.OnPlayerReset.Subscribe(eventHandler,HandlePlayerReset);
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerReset.Subscribe(eventHandler, HandlePlayerReset);
    }


    public void IntroStarted()
    {
        OnIntroStarted.TryInvoke();
    }
    public void IntroFinished()
    {
        OnIntroFinished.TryInvoke();
    }

    private async void HandlePlayerReset()
    {
        await Task.Delay(GameManager.currentGameManager.resetFreezeDurationMs);
        if (playIntro) director.Play();
    }
}
