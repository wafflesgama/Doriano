using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Sailor : MonoBehaviour, Interactable
{

    public Vector3 offset;
    public Vector3 GetOffset() => offset;

    [Multiline] public string[] introDialogue;

    [Multiline] public string[] noFoundDialogue;
    [Multiline] public string[] someFoundDialogue;
    [Multiline] public string[] foundDialogue;

    bool hasMadeIntro;

    UEventHandler eventHandler = new UEventHandler();
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

            UIManager.OnStartedDialogue.TryInvoke(transform, introDialogue);
        }
        gameObject.tag = "Untagged";
    }

    public async void FinishedDialogue()
    {
        await Task.Delay(1000);
        gameObject.tag = "Interactable";
    }
}
