using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UEventHandler;

public class InteractionHandler : MonoBehaviour
{

    public static UEvent<Transform, Vector3> OnInteractableAppeared = new UEvent<Transform, Vector3>();
    public static UEvent OnInteractableDisappeared = new UEvent();
    public static UEvent<Vector3> OnSplash = new UEvent<Vector3>();
    static GameObject objectToInteract = null;

    public InputManager inputManager;


    UEventHandler eventHandler = new UEventHandler();

    private void Start()
    {
        inputManager.input_attack.Onpressed.Subscribe(eventHandler, TryInteract);
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }
    public static bool IsInteractableNearby() => objectToInteract != null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null) return;

        if (other.transform.parent.tag == "Water")
        {
            OnSplash.TryInvoke(other.ClosestPoint(transform.position));
            return;
        }


        if (other.transform.parent.tag == "Interactable")
        {
            if (objectToInteract == null || objectToInteract.transform.position != other.transform.position)
            {
                objectToInteract = other.gameObject;
                if (objectToInteract.transform.parent.TryGetComponent<Interactable>(out Interactable interactable))
                    OnInteractableAppeared.TryInvoke(other.transform, interactable.GetOffset());
            }
        }
        else if (other.transform.parent.tag == "KillBound")
        {
            GameManager.currentGameManager.ResetPlayer();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == null) return;

        //if (other.transform.parent.tag == "Interactable" || objectToInteract.transform.parent.tag != "Uninteractable")
        //{
            if (objectToInteract != null && objectToInteract.transform.position == other.transform.position && objectToInteract.name== other.gameObject.name)
            {
                objectToInteract = null;
                OnInteractableDisappeared.TryInvoke();
            }
        //}
    }

    public void TryInteract()
    {
        if (objectToInteract == null || objectToInteract.transform.parent.tag != "Interactable") return;
        if (PlayerCutsceneManager.isInCutscene) return;

        objectToInteract.transform.parent.GetComponent<Interactable>().Interact();
        OnInteractableDisappeared.TryInvoke();
    }

}
