using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading.Tasks;
using Uevents;

public class CameraController : MonoBehaviour
{
    public enum ViewType
    {
        Unset,
        MainView,
        LockView,
        DialogueView
    }
    public Camera cameraBrain;
    public CinemachineVirtualCamera mainViewCamera;
    public CinemachineVirtualCamera lockViewCamera;
    public CinemachineVirtualCamera dialogueCamera;

    Transform dialogueTarget;
    ViewType prevView = ViewType.Unset, currentView = ViewType.Unset;
    UeventHandler eventHandler = new UeventHandler();

    void Start()
    {
        SwitchView(ViewType.MainView);

        UIManager.OnStartedDialogue.Subscribe(eventHandler, StartDialogueCamera);
        UIManager.OnFinishedDialogue.Subscribe(eventHandler, EndDialogueCamera);
        PlayerCutsceneManager.OnEndingStarted.Subscribe(eventHandler, DisableCameras);
    }
    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void DisableCameras()
    {
        await Task.Delay(1000);
        cameraBrain.gameObject.SetActive(false);
    }

    public void SwitchView(ViewType viewType)
    {
        if (prevView == ViewType.Unset)
            prevView = viewType;
        else
            prevView = currentView;

        currentView = viewType;

        mainViewCamera.Priority = 10;
        lockViewCamera.Priority = 10;
        dialogueCamera.Priority = 10;

        //Debug.LogWarning("Switching view");
        switch (viewType)
        {
            case ViewType.LockView:
                lockViewCamera.Priority = 11;
                break;
            case ViewType.DialogueView:
                dialogueCamera.Priority = 11;
                break;
            default:
                mainViewCamera.Priority = 11;
                break;
        }

    }
    public void SwitchToPrevView() => SwitchView(prevView);

    private void StartDialogueCamera(Transform t, string[] m)
    {
        dialogueTarget = t;
        dialogueCamera.LookAt = dialogueTarget;
        dialogueCamera.Follow = dialogueTarget;
        SwitchView(ViewType.DialogueView);
    }

    private void EndDialogueCamera()
    {
        SwitchToPrevView();
    }


}
