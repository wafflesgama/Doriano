using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UEventHandler;

public class GameManager : MonoBehaviour
{
    public static GameManager currentGameManager;
    public static UEvent OnExitScreen = new UEvent();
    //public static Action OnExitScreen;
    public static UEvent OnPlayerReset = new UEvent();
    //public static Action OnPlayerReset;

    public int resetFreezeDurationMs = 800;

    bool restarFlag;

    private void Awake()
    {
        currentGameManager = this;
    }

    private void OnDestroy()
    {
        if (currentGameManager == this)
            currentGameManager = null;

    }

    public async void ResetPlayer()
    {
        OnPlayerReset.TryInvoke();
        //OnPlayerReset?.Invoke();
        await Task.Delay(700);

    }

    public async void RestartScene()
    {
        if (restarFlag) return;
        restarFlag = true;

        OnExitScreen.TryInvoke();
        //OnExitScreen.Invoke();
        await Task.Delay(700);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public async void MainMenu()
    {
        OnExitScreen.TryInvoke();
        //OnExitScreen?.Invoke();
        await Task.Delay(700);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public async void ExitGame()
    {
        OnExitScreen.TryInvoke();
        //OnExitScreen?.Invoke();
        await Task.Delay(700);
        Application.Quit();
    }
}
