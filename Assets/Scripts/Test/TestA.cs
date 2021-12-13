using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UEventHandler;

public class TestA : MonoBehaviour
{
    //public EventManager eventManager= new EventManager();
    public UEvent OnSpaceEnter=new UEvent();
    void Start()
    {
        //OnSpaceEnter=new UEvent(ref eventManager);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpaceEnter.TryInvoke();
        }
    }
}
