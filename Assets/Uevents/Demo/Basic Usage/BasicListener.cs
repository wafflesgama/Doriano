using Uevents;
using UnityEngine;
using UnityEngine.UI;

public class BasicListener : MonoBehaviour
{
    public Text outputText;

    //Event Handler (per listener component)
    private UeventHandler eventHandler = new UeventHandler();

    void Start()
    {
        BasicInvoker.staticZeroEvent.Subscribe(eventHandler, AnswerZeroInvoke);
        BasicInvoker.staticOneEvent.Subscribe(eventHandler, AnswerOneInvoke);
    }

    private void AnswerZeroInvoke()
    {
        outputText.text = "Event answered;\n" + outputText.text;
    }

    private void AnswerOneInvoke(string data)
    {
        outputText.text = $"Event answered ({data});\n" + outputText.text;
    }


    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }
}
