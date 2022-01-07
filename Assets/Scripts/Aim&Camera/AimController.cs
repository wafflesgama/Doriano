using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uevents;

//public delegate void ChangeView();

public class AimController : MonoBehaviour
{

    public Uevent OnLockTarget = new Uevent();
    //public event ChangeView OnLockTarget;
    public Uevent OnUnlockTarget = new Uevent();
    //public event ChangeView OnUnlockTarget;
    public InputManager inputManager;
    public ObjectLockController lockController;
    public CameraController cameraController;
    public Transform aimTarget;
    public Transform playerSource;

    public float xSensitivity = 1;
    public float ySensitivity = 1;

    [Header("Exposed Variables")]
    public LockableObject currentLockedObj;
    public UeventHandler eventHandler = new UeventHandler();

    bool isAimFrozen,pausedState;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputManager.input_lockView.Onpressed.Subscribe(eventHandler, TryLock);
        inputManager.input_lockView.Onreleased.Subscribe(eventHandler, TryUnlock);
        PauseHandler.OnPause.Subscribe(eventHandler, () => PauseHandle(pause: true));
        PauseHandler.OnUnpause.Subscribe(eventHandler, () => PauseHandle(pause: false));
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }


    void TryLock()
    {
        currentLockedObj = lockController.TrySelectLockObj(aimTarget.position, aimTarget.forward);

        if (currentLockedObj != null)
        {
            cameraController.SwitchView(CameraController.ViewType.LockView);
            OnLockTarget.TryInvoke();
        }
    }

    void TryUnlock()
    {
        currentLockedObj = null;
        cameraController.SwitchView(CameraController.ViewType.MainView);
        OnUnlockTarget.TryInvoke();
    }

    void PauseHandle(bool pause)
    {
        pausedState = pause;
        isAimFrozen = pause;
        Cursor.visible = pause;
        Cursor.lockState = !pause ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void Update()
    {
        if (isAimFrozen) return;

        if (!pausedState && Cursor.visible)
        {
            Cursor.visible = false;
            Cursor.lockState =  CursorLockMode.Locked;
        }

        if (currentLockedObj != null)
        {
            aimTarget.rotation = Quaternion.LookRotation(currentLockedObj.transform.position - aimTarget.position);
        }
        else
        {

            // var verticalRotation=Mathf.Lerp(lastAimValue,-inputManager.input_look.value.y,Time.deltaTime*.5f);
            // Debug.LogWarning("Input val  X:"  + inputManager.input_look.value.x + " Y:"+ inputManager.input_look.value.y);
            var newRotation = Quaternion.Euler(aimTarget.eulerAngles.x + (inputManager.input_look.value.y * ySensitivity * -.08f),
                                                aimTarget.eulerAngles.y + (inputManager.input_look.value.x * xSensitivity * .08f),
                                                aimTarget.eulerAngles.z);

            // Debug.Log("Rotation is X:" + newRotation.eulerAngles.x +
            //                                             " Y:" + newRotation.eulerAngles.y +
            //                                               " Z:" + newRotation.eulerAngles.z);

            aimTarget.rotation = newRotation;
        }
        // aimTarget.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * .5f);

        // lastAimValue= inputManager.input_look.value.y;
    }
}
