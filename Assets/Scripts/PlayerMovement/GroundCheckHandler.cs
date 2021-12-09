using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckHandler : MonoBehaviour
{
    private List<string> contacts;
    void Start()
    {
        contacts = new List<string>();
    }

    public bool IsGrounded() => contacts.Count > 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!contacts.Contains(other.name))
            contacts.Add(other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (contacts.Contains(other.name))
            contacts.Remove(other.name);
    }
}
