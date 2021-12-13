using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Sailor : MonoBehaviour, Interactable 
{

    public Vector3 offset;
    public Vector3 GetOffset() => offset;

    [Multiline] public string[] introDialogue;

    UEventHandler eventHandler = new UEventHandler();
    void Start()
    {
        UIManager.OnFinishedDialogue.Subscribe(eventHandler,FinishedDialogue);
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }

    public void Interact()
    {
        UIManager.OnStartedDialogue.TryInvoke(transform,introDialogue);
        gameObject.tag = "Untagged";
    }

    public async void FinishedDialogue()
    {
        await Task.Delay(1000);
        gameObject.tag = "Interactable";
    }
}
