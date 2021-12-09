using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public delegate void InteractableNotify(Vector3 pos);

    public static InteractableNotify OnInteractableAppeared;
    public static Action OnInteractableDisappeared;
    static GameObject objectToInteract = null;

    public InputManager inputManager;


    private void Start()
    {
        inputManager.input_attack.Onpressed += TryInteract;
    }

    private void OnDestroy()
    {
        inputManager.input_attack.Onpressed -= TryInteract;
    }
    public static bool IsInteractableNearby() => objectToInteract != null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.tag == "Interactable")
        {
            if (objectToInteract == null || objectToInteract.transform.position != other.transform.position)
            {
                objectToInteract = other.gameObject;
                var offset = objectToInteract.transform.parent.GetComponent<Interactable>().GetOffset();
                OnInteractableAppeared?.Invoke(other.transform.position + offset);
            }
        }else if(other.transform.parent.tag == "KillBound")
        {
            GameManager.currentGameManager.ResetPlayer();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.tag == "Interactable")
        {
            if (objectToInteract != null && objectToInteract.transform.position == other.transform.position)
            {
                objectToInteract = null;
                OnInteractableDisappeared?.Invoke();
            }
        }
    }

    public void TryInteract()
    {
        if (objectToInteract == null || objectToInteract.transform.parent.tag != "Interactable") return;

        objectToInteract.transform.parent.GetComponent<Interactable>().Interact();
        OnInteractableDisappeared?.Invoke();
    }

}
