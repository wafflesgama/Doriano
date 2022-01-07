using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Uevents;

public class Sailor : MonoBehaviour, Interactable
{

    public Vector3 offset;
    public Vector3 GetOffset() => offset;

    [Multiline] public string[] introDialogue;

    [Multiline] public string[] noFoundDialogue;
    [Multiline] public string[] foundDialogue;

    bool hasMadeIntro;

    UeventHandler eventHandler = new UeventHandler();
    void Start()
    {
        UIManager.OnFinishedDialogue.Subscribe(eventHandler, FinishedDialogue);
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }

    public void Interact()
    {
        if (!hasMadeIntro)
        {
            hasMadeIntro = true;
            UIManager.OnStartedDialogue.TryInvoke(transform, introDialogue);
        }
        else
        {
            if (GameManager.currentGameManager.collectedItems.Count == 0)
            {
                UIManager.OnStartedDialogue.TryInvoke(transform, noFoundDialogue);
            }
            else if (GameManager.currentGameManager.collectedItems.Count < GameManager.currentGameManager.necessaryItems)
            {
                string[] someFoundDialogue = new string[4];
                someFoundDialogue[0] = "Oh!";

                someFoundDialogue[1] = "You found a ";
                foreach (var item in GameManager.currentGameManager.collectedItems)
                    someFoundDialogue[1] += "<#DE4D4D>" + item + "</color> and a ";
                someFoundDialogue[1] = someFoundDialogue[1].Substring(0, someFoundDialogue[1].Length - 7);
                someFoundDialogue[2] = "Try finding the other parts around here";
                someFoundDialogue[3] = "Maybe all hope is not lost, afterall";
                UIManager.OnStartedDialogue.TryInvoke(transform, someFoundDialogue);
            }
            else
            {
                UIManager.OnStartedDialogue.TryInvoke(transform, foundDialogue);
                UIManager.OnFinishedDialogue.Subscribe(eventHandler, () =>
                 {
                     PlayerCutsceneManager.currentPlayerCutsceneManager.StartEnding();
                 });
            }
        }
        gameObject.tag = "Uninteractable";
    }

    private async void FinishedDialogue()
    {
        await Task.Delay(1000);
        gameObject.tag = "Interactable";
    }
}
