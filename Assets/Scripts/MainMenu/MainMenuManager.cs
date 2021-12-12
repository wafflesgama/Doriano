using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class MainMenuManager : MonoBehaviour
{
    public Image fade;
    public float fadeSpeed = .5f;
    public AudioSource audioSource;
    public AudioClip clickSound;

    bool inTransition;
    void Awake()
    {
        fade.color = Color.black;
        FadeScreen(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && !inTransition)
        {
            inTransition = true;
            audioSource.PlayOneShot(clickSound);
            GoToGame();
        }
    }

    private async void GoToGame()
    {
        FadeScreen(true);
        await Task.Delay(600);
        SceneManager.LoadScene("World1");
    }
    private void FadeScreen(bool fadeIn) => fade.DOFade(fadeIn ? 1 : 0, fadeSpeed).SetEase(Ease.InOutQuad);
}
