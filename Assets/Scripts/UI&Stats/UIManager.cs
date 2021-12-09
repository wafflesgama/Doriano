using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public delegate void DialogueAction(Transform narrator,string[] messages);
    public static DialogueAction OnStartedDialogue;
    public static Action OnFinishedDialogue;

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
    public float showInteractAnimDuration=.5f;
    public Ease showInteractEase;

    [Header("Item Showcase")]
    public int messageDurationMs=2500;
    public float inAnimDuration=1;
    public Ease inAnimEase;
    public Ease outAnimEase;
    public RectTransform itemContainer;
    public Image itemImage;
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;


    int lockArrowShowing = -1;
    bool isInDialogue;

    private void Awake()
    {
        fadeAnimator.gameObject.SetActive(true);
    }

    void Start()
    {
        aimController.OnLockTarget += ShowArrow;
        aimController.OnUnlockTarget += HideArrow;
        Chest.OnChestItemShow += ShowItem;
        InteractionHandler.OnInteractableAppeared += ShowInteractable;
        InteractionHandler.OnInteractableDisappeared += HideInteractable;
        OnStartedDialogue += RegisterDialogue;
        inputManager.input_attack.Onpressed += TryNextMessage;
        GameManager.OnLevelRestart += () => FadeScreen(fadeIn: false);
        GameManager.OnPlayerReset += ResetPlayerUI;

    }


    private void OnDestroy()
    {
        aimController.OnLockTarget -= ShowArrow;
        aimController.OnUnlockTarget -= HideArrow;
        Chest.OnChestItemShow -= ShowItem;
        InteractionHandler.OnInteractableAppeared -= ShowInteractable;
        InteractionHandler.OnInteractableDisappeared -= HideInteractable;
        OnStartedDialogue -= RegisterDialogue;
        inputManager.input_attack.Onpressed -= TryNextMessage;
        GameManager.OnLevelRestart -= () => FadeScreen(fadeIn: false);
        GameManager.OnPlayerReset -= ResetPlayerUI;
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
    }

    

    private void FadeScreen(bool fadeIn) => fadeAnimator.SetBool("FadeIn", fadeIn);


    private void ShowArrow()
    {
        lockArrowShowing = 1;
        //var targetPos = aimController.currentLockedObj.position+new Vector3(0,1,0);
        //arrowIcon.transform.position = targetPos;
        //arrowIcon.color =  Color.red;
    }

    private void HideArrow()
    {
        lockArrowShowing = 0;
        //arrowIcon.color = new Color(0, 0, 0, 0);
    }


    private void ShowInteractable(Vector3 position)
    {
        interactableText.transform.position = position;
        interactableText.transform.DOScale(Vector3.one, showInteractAnimDuration).SetEase(showInteractEase);
    }

    private void HideInteractable()
    {
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

    private async void RegisterDialogue(Transform t,string[] dialogue)
    {
        dialogueContainer.DOScale(1, dialogueAnimDuration).SetEase(dialogueInEase);
        await Task.Delay((int)dialogueAnimDuration*1000/2);
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
            OnFinishedDialogue?.Invoke();
        }
    }

    private bool NextMessage() => dialogueWriter.WriteNextMessage();


    private async void ResetPlayerUI()
    {
        FadeScreen(false);
        await Task.Delay(GameManager.currentGameManager.resetFreezeDurationMs);
        FadeScreen(true);
    }


}
