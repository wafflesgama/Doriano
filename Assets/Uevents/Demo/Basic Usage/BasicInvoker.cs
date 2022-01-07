using Uevents;
using UnityEngine;
using UnityEngine.UI;

public class BasicInvoker : MonoBehaviour
{
    //Static Events
    public static Uevent staticZeroEvent = new Uevent();
    public static Uevent<string> staticOneEvent = new Uevent<string>();


    void Start()
    {
    }

    public void InvokeZeroEvent()
    {
        staticZeroEvent.TryInvoke();
    }

    public void InvokeParamEvent(InputField input)
    {
        staticOneEvent.TryInvoke(input.text);

    }



}
