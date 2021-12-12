using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

public class PauseHandler : MonoBehaviour
{
    public InputManager inputManager;
    public static Action OnPause;
    public static Action OnUnpause;

    bool isInPause;

    private void Awake()
    {
        inputManager.input_pause.Onpressed += PauseHandle;
    }

    public void PauseHandle()
    {
        PlayerSoundManager.currentManager.PlayUIClick();

        if (isInPause)
            OnUnpause?.Invoke();
        else
            OnPause?.Invoke();

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
        OnUnpause?.Invoke();
        GameManager.currentGameManager.ResetPlayer();
    }

    public void ToMainMenu()
    {
        PlayerSoundManager.currentManager.PlayUIClick();
        GameManager.currentGameManager.MainMenu();
    }

}
