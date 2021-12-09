using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCutsceneManager : MonoBehaviour
{
    public static Action OnIntroFinished;
    public static Action OnIntroStarted;
    public static bool isIntroEnabled;

    PlayableDirector director;
    public bool playIntro = true;

    private void Awake()
    {
        isIntroEnabled = playIntro;
        director = GetComponent<PlayableDirector>();
        if (playIntro) director.Play();
    }
    void Start()
    {
        GameManager.OnPlayerReset += HandlePlayerReset;
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerReset -= HandlePlayerReset;
    }


    public void IntroStarted()
    {
        OnIntroStarted?.Invoke();
    }
    public void IntroFinished()
    {
        OnIntroFinished?.Invoke();
    }

    private async void HandlePlayerReset()
    {
        await Task.Delay(GameManager.currentGameManager.resetFreezeDurationMs);
        if (playIntro) director.Play();
    }
}
