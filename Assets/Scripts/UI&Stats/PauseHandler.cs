using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using static UEventHandler;

public class PauseHandler : MonoBehaviour
{
    public InputManager inputManager;
    //public static Action OnPause;
    //public static Action OnUnpause;
    public static UEvent OnPause= new UEvent();
    public static UEvent OnUnpause= new UEvent();

    bool isInPause;

    UEventHandler eventHandler = new UEventHandler();

    private void Awake()
    {
    }

    private void Start()
    {
        inputManager.input_pause.Onpressed.Subscribe(eventHandler,PauseHandle);
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }

    public void PauseHandle()
    {
        PlayerSoundManager.currentManager.PlayUIClick();

        if (isInPause)
            OnUnpause.TryInvoke();
        //OnUnpause?.Invoke();
        else
            OnPause.TryInvoke();
        //OnPause?.Invoke();

        isInPause = !isInPause;
    }

    public void ExitGame()
    {
        PlayerSoundManager.currentManager.PlayUIClick();
        GameManager.currentGameManager.ExitGame();
    }

    public void RestartLevel()
    {
        PlayerSoundManager.currentManager.PlayUIClick();
        OnUnpause.TryInvoke();
        GameManager.currentGameManager.ResetPlayer();
    }

    public void ToMainMenu()
    {
        Cursor.visible = false;
        PlayerSoundManager.currentManager.PlayUIClick();
        GameManager.currentGameManager.GoToMainMenu();
    }

}
