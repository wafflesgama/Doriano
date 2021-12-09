using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ChangeView();

public class AimController : MonoBehaviour
{

    public event ChangeView OnLockTarget;
    public event ChangeView OnUnlockTarget;
    public InputManager inputManager;
    public ObjectLockController lockController;
    public CameraController cameraController;
    public Transform aimTarget;
    public Transform playerSource;

    public float xSensitivity = 1;
    public float ySensitivity = 1;


    public LockableObject currentLockedObj;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputManager.input_lockView.Onpressed += TryLock;
        inputManager.input_lockView.Onreleased += TryUnlock;
    }

    private void OnDestroy()
    {
        inputManager.input_lockView.Onpressed -= TryLock;
        inputManager.input_lockView.Onreleased -= TryUnlock;
    }


    void TryLock()
    {
        currentLockedObj = lockController.TrySelectLockObj(aimTarget.position, aimTarget.forward);

        if (currentLockedObj != null)
        {
            cameraController.SwitchView(CameraController.ViewType.LockView);
            OnLockTarget.Invoke();
        }
    }

    void TryUnlock()
    {
        currentLockedObj = null;
        cameraController.SwitchView(CameraController.ViewType.MainView);
        OnUnlockTarget.Invoke();
    }

    void Update()
    {
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
