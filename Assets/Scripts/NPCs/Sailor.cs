using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Sailor : MonoBehaviour, Interactable 
{

    public Vector3 offset;
    public Vector3 GetOffset() => offset;

    [Multiline] public string[] introDialogue;

    void Start()
    {
        UIManager.OnFinishedDialogue += FinishedDialogue;
    }

    private void OnDestroy()
    {
        UIManager.OnFinishedDialogue -= FinishedDialogue;
    }

    public void Interact()
    {
        UIManager.OnStartedDialogue?.Invoke(transform,introDialogue);
        gameObject.tag = "Untagged";
    }

    public async void FinishedDialogue()
    {
        await Task.Delay(1000);
        gameObject.tag = "Interactable";
    }
}
