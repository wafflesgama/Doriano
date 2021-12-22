using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GenericNPC : MonoBehaviour,Interactable
{
    public Vector3 offset;
    [Multiline] public string[] dialogue;

    UEventHandler eventHandler = new UEventHandler();
    void Start()
    {
        UIManager.OnFinishedDialogue.Subscribe(eventHandler, FinishedDialogue);
    }

    public Vector3 GetOffset() => offset;

    public void Interact()
    {
        UIManager.OnStartedDialogue.TryInvoke(transform, dialogue);
        gameObject.tag = "Untagged";
    }


    private async void FinishedDialogue()
    {
        await Task.Delay(1000);
        gameObject.tag = "Interactable";
    }
}
