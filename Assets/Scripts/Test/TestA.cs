using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uevents;

public class TestA : MonoBehaviour
{
    //public EventManager eventManager= new EventManager();
    public Uevent OnSpaceEnter=new Uevent();
    void Start()
    {
        //OnSpaceEnter=new Uevent(ref eventManager);
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
