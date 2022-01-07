using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Uevents;

public class GameManager : MonoBehaviour
{
    public static GameManager currentGameManager;
    public static Uevent OnExitScreen = new Uevent();
    public static Uevent OnPlayerReset = new Uevent();

    public int resetFreezeDurationMs = 800;

    public int necessaryItems = 4;
    public List<string> collectedItems;
    public int gumpKilled=0;
    bool restarFlag;

    public UeventHandler eventHandler = new UeventHandler(); 

    private void Awake()
    {
        currentGameManager = this;
        collectedItems = new List<string>();
        gumpKilled = 0;
        Gump.OnGumpDied.Subscribe(eventHandler, () => gumpKilled++);
    }

    private void OnDestroy()
    {
        if (currentGameManager == this)
            currentGameManager = null;
    }

    public void ItemCollected(string item)
    {
        collectedItems.Add(item);   
    }

    #region Level & Scene Mang

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

    public async void GoToMainMenu()
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

    #endregion Level & Scene Mang
}
