using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using System;
using  Uevents;

public class UIManager : MonoBehaviour
{
    public static Uevent<Transform, string[]> OnStartedDialogue = new Uevent<Transform, string[]>();
    public static Uevent OnFinishedDialogue = new Uevent();
    public static Uevent<bool> OnFadeScreen = new Uevent<bool>();

    public AimController aimController;
    public InputManager inputManager;

    [Header("Dialogue")]
    public DialogueWriter dialogueWriter;
    public RectTransform dialogueContainer;
    public float dialogueAnimDuration = .5f;
    public Ease dialogueInEase;
    public Ease dialogueOutEase;
    public Image continueIcon;

    [Header("Fade")]
    public Animator fadeAnimator;

    [Header("Arrow")]
    public Image arrowIcon;

    [Header("Interactable")]
    public TextMeshProUGUI interactableText;
    public float showInteractAnimDuration = .5f;
    public Ease showInteractEase;

    [Header("Item Showcase")]
    public int messageDurationMs = 2500;
    public float inAnimDuration = 1;
    public Ease inAnimEase;
    public Ease outAnimEase;
    public RectTransform itemContainer;
    public Image itemImage;
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;

    [Header("Pause Menu")]
    public CanvasGroup pauseGroup;
    public UnityEngine.EventSystems.EventSystem eventSystem;
    public float pauseAnimDuration = .15f;

    [Header("Gump Counter")]
    public RectTransform gumpCounterGroup;
    public TextMeshProUGUI gumpCounterText;

    [Header("Credits")]
    public DialogueWriter creditsWriter;
    [Multiline]
    public string[] creditsMessage;
    public CanvasGroup creditsGroup;

    public UeventHandler eventHandler = new UeventHandler();
    int lockArrowShowing = -1;
    bool isInDialogue;
    int gumpShowCounter = 0;

    bool trackInteractable;
    Transform interactableRef;
    Vector3 interactableOffset;
    void Start()
    {
        fadeAnimator.gameObject.SetActive(true);
        OnFadeScreen.TryInvoke(true);

        aimController.OnLockTarget.Subscribe(eventHandler, ShowArrow);
        aimController.OnUnlockTarget.Subscribe(eventHandler, HideArrow);
        Chest.OnChestItemShow.Subscribe(eventHandler, ShowItem);
        InteractionHandler.OnInteractableAppeared.Subscribe(eventHandler, ShowInteractable);
        InteractionHandler.OnInteractableDisappeared.Subscribe(eventHandler, HideInteractable);
        OnStartedDialogue.Subscribe(eventHandler, RegisterDialogue);
        inputManager.input_attack.Onpressed.Subscribe(eventHandler, TryNextMessage);
        GameManager.OnExitScreen.Subscribe(eventHandler, () => FadeScreen(fadeIn: false));
        GameManager.OnPlayerReset.Subscribe(eventHandler, () => FadeInOutScreen());
        PauseHandler.OnPause.Subscribe(eventHandler, () => FadePauseMenu(fadeIn: true));
        PauseHandler.OnUnpause.Subscribe(eventHandler, () => FadePauseMenu(fadeIn: false));
        PlayerCutsceneManager.OnEndingStarted.Subscribe(eventHandler, () => FadeScreen(fadeIn: false));
        PlayerCutsceneManager.OnEndingFadeIn.Subscribe(eventHandler, () => FadeScreen(fadeIn: true));
        PlayerCutsceneManager.OnCreditsStarted.Subscribe(eventHandler, ShowCredits);
        Gump.OnGumpDied.Subscribe(eventHandler, ShowGumpCounter);
    }


    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }



    // Update is called once per frame
    void Update()
    {
        if (lockArrowShowing == 1)
        {
            var targetPos = aimController.currentLockedObj.transform.position + aimController.currentLockedObj.displayOffset;
            arrowIcon.transform.position = targetPos;
            arrowIcon.color = Color.red;
        }
        else if (lockArrowShowing == 0)
        {
            arrowIcon.color = new Color(0, 0, 0, 0);
            lockArrowShowing = -1;
        }

        if(trackInteractable)
            interactableText.transform.position = interactableRef.position+interactableOffset;

    }



    private void FadeScreen(bool fadeIn)
    {
        OnFadeScreen.TryInvoke(fadeIn);
        //OnFadeScreen?.Invoke(fadeIn);
        fadeAnimator.SetBool("FadeIn", fadeIn);
    }


    private void ShowArrow()
    {
        lockArrowShowing = 1;
    }

    private void HideArrow()
    {
        lockArrowShowing = 0;
    }


    private void ShowInteractable(Transform source,Vector3 offset)
    {
        trackInteractable=true;
        interactableRef = source;
        interactableOffset = offset;
        interactableText.transform.DOScale(Vector3.one, showInteractAnimDuration).SetEase(showInteractEase);
    }

    private void HideInteractable()
    {
        trackInteractable = false;
        interactableText.transform.DOScale(Vector3.zero, showInteractAnimDuration).SetEase(showInteractEase);
    }

    private async void ShowItem(Sprite image, string name, string description)
    {
        itemContainer.DOScale(1, inAnimDuration).SetEase(inAnimEase);
        itemImage.sprite = image;
        itemTitle.text = name;
        itemDescription.text = description;

        await Task.Delay(messageDurationMs);
        itemContainer.DOScale(0, inAnimDuration).SetEase(outAnimEase);

    }

    private async void RegisterDialogue(Transform t, string[] dialogue)
    {
        dialogueContainer.DOScale(1, dialogueAnimDuration).SetEase(dialogueInEase);
        await Task.Delay((int)dialogueAnimDuration * 1000 / 2);
        dialogueWriter.RegisterMessages(dialogue);
        NextMessage();
        await Task.Delay(500);
        isInDialogue = true;
    }

    private void TryNextMessage()
    {
        if (!isInDialogue) return;

        //continueIcon.gameObject.SetActive(false);
        if (!NextMessage())
        {
            isInDialogue = false;
            dialogueContainer.DOScale(0, dialogueAnimDuration).SetEase(dialogueOutEase);
            OnFinishedDialogue.TryInvoke();
        }
    }

    private bool NextMessage() => dialogueWriter.WriteNextMessage();


    private async void FadeInOutScreen(int? delay = null)
    {
        FadeScreen(false);
        await Task.Delay(delay.HasValue ? delay.Value : GameManager.currentGameManager.resetFreezeDurationMs);
        FadeScreen(true);
    }

    private void FadePauseMenu(bool fadeIn)
    {
        if (fadeIn)
            eventSystem.SetSelectedGameObject(null);

        pauseGroup.interactable = fadeIn;
        pauseGroup.blocksRaycasts = fadeIn;
        pauseGroup.DOFade(fadeIn ? 1 : 0, pauseAnimDuration).SetEase(fadeIn ? Ease.OutQuart : Ease.InQuart);
    }

    private async void ShowGumpCounter()
    {
        await Task.Delay(50);
        gumpCounterText.text = GameManager.currentGameManager.gumpKilled + "x";
        gumpShowCounter++;
        if (gumpShowCounter <= 1)
            gumpCounterGroup.transform.DOLocalMoveY(-100, pauseAnimDuration).SetEase(Ease.OutBack);

        await Task.Delay(2000);
        gumpShowCounter--;

        if (gumpShowCounter == 0)
            gumpCounterGroup.transform.DOLocalMoveY(100, pauseAnimDuration).SetEase(Ease.InBack);
    }

    private  void ShowEnding()
    {
        FadeInOutScreen(5000);
    }

    private async void ShowCredits()
    {
        creditsGroup.DOFade(1, 1.4f).SetEase(Ease.InQuad);
        await Task.Delay(1500);
        await creditsWriter.WriteAllMessages(creditsMessage, 4000);
        await Task.Delay(3000);
        GameManager.currentGameManager.GoToMainMenu();
    }


}
