using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InputSysFix : MonoBehaviour
{
    
    private void Start()
    {
        HackFixForEditorPlayModeDelay();
    }
    void HackFixForEditorPlayModeDelay()
    {
#if UNITY_EDITOR
        // Using reflection, does this: InputSystem.s_SystemObject.exitEditModeTime = 0

        // Get InputSystem.s_SystemObject object
        FieldInfo systemObjectField = typeof(UnityEngine.InputSystem.InputSystem).GetField("s_SystemObject", BindingFlags.NonPublic | BindingFlags.Static);
        object systemObject = systemObjectField.GetValue(null);

        // Get InputSystemObject.exitEditModeTime field
        Assembly inputSystemAssembly = typeof(UnityEngine.InputSystem.InputSystem).Assembly;
        Type inputSystemObjectType = inputSystemAssembly.GetType("UnityEngine.InputSystem.InputSystemObject");
        FieldInfo exitEditModeTimeField = inputSystemObjectType.GetField("exitEditModeTime", BindingFlags.Public | BindingFlags.Instance);

        // Set exitEditModeTime to zero
        exitEditModeTimeField.SetValue(systemObject, 0d);
#endif
    }
}
