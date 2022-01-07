using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Uevents;

public class GenericNPC : MonoBehaviour,Interactable
{
    public Vector3 offset;
    [Multiline] public string[] dialogue;

    UeventHandler eventHandler = new UeventHandler();
    void Start()
    {
        UIManager.OnFinishedDialogue.Subscribe(eventHandler, FinishedDialogue);
    }

    public Vector3 GetOffset() => offset;

    public void Interact()
    {
        UIManager.OnStartedDialogue.TryInvoke(transform, dialogue);
        gameObject.tag = "Uninteractable";
    }


    private async void FinishedDialogue()
    {
        await Task.Delay(1000);
        gameObject.tag = "Interactable";
    }
}
