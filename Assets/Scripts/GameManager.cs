using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager currentGameManager;
    public static Action OnLevelRestart;
    public static Action OnPlayerReset;

    public int resetFreezeDurationMs=800;

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
        OnPlayerReset?.Invoke();
        await Task.Delay(700);

    }

    public async void RestartScene()
    {
        if (restarFlag) return;
        restarFlag = true;

        OnLevelRestart?.Invoke();
        await Task.Delay(700);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
