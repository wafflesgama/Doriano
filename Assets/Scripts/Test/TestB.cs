using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uevents;

public class TestB : MonoBehaviour
{
    public UeventHandler eventHandler=new UeventHandler();

    TestA a;
    // Start is called before the first frame update
    void Start()
    {
        a=GetComponent<TestA>();

        a.OnSpaceEnter.Subscribe(eventHandler, Report);
        a.OnSpaceEnter.Subscribe(eventHandler, () => { Debug.Log("Reporting from anon class"); });
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }

    public void Report()
    {
        Debug.Log("Reporting from defined class");
    }
}
