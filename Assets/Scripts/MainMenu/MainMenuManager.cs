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
    public CanvasGroup mainGroup;
    public CanvasGroup creditsGroup;
    public CanvasGroup settingsGroup;
    public Animator fadeAnimator;

    //int menuState = -1;
    //bool inTransition;
    CanvasGroup currentGroup;
    void Awake()
    {
        //fade.color = Color.black;
        currentGroup = mainGroup;
    }

    private void Start()
    {
        Cursor.visible = true;
    }


    //// Update is called once per frame
    //void Update()
    //{
    //    //if (Input.anyKey && menuState == -1)
    //    //{
    //    //    menuState = 0;
    //    //    audioSource.PlayOneShot(clickSound);
    //    //    ChangeScreen(mainGroup);
    //    //}
    //}

    public void GoToMain() => ChangeScreen(mainGroup);
    public void GoToSettings() => ChangeScreen(settingsGroup);
    public void GoToCredits() => ChangeScreen(creditsGroup);

    public async void GoToGame()
    {
        Cursor.visible = false;
        audioSource.PlayOneShot(clickSound);
        fadeAnimator.SetBool("FadeIn", false);
        await Task.Delay(800);
        SceneManager.LoadScene("World1");
    }
    //private void FadeScreen(bool fadeIn) => fade.DOFade(fadeIn ? 1 : 0, fadeSpeed).SetEase(Ease.InOutQuad);


    private async void ChangeScreen(CanvasGroup outGroup)
    {
        audioSource.PlayOneShot(clickSound);
        fadeAnimator.SetBool("FadeIn", false);
        await Task.Delay(800);
        currentGroup.gameObject.SetActive(false);
        outGroup.gameObject.SetActive(true);
        currentGroup = outGroup;
        fadeAnimator.SetBool("FadeIn", true);
    }
}
