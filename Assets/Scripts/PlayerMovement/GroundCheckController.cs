using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckController : MonoBehaviour
{
    public List<int> collidersOnContact = new List<int>();

    public Transform groundHeightRef;

    public bool IsGrounded() => collidersOnContact.Count > 0;

    private void OnCollisionEnter(Collision other)
    {
        //Debug.LogError("OnCollisionEnter");
        ContactPoint[] contacts = new ContactPoint[250];
        var contactCount = other.GetContacts(contacts);

        if (contactCount <= 0 || contacts == null) return;

        bool isContactBelowGround = false;

        for (int i = 0; i < contactCount; i++)
        {
            if (contacts[i].point.y >= groundHeightRef.position.y) continue;
            isContactBelowGround = true;
            break;
        }

        if (!isContactBelowGround) return;

        var id = other.collider.GetInstanceID();
        if (!collidersOnContact.Contains(id))
            collidersOnContact.Add(id);
    }


    private void OnCollisionExit(Collision other)
    {
        var id = other.collider.GetInstanceID();
        if (collidersOnContact.Contains(id))
            collidersOnContact.Remove(id);
    }
}
