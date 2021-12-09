using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLockController : MonoBehaviour
{
    public Transform lockDistanceRef;
    public LayerMask layerMask;
    public float minimumLockThreshold=.7f;

    public LockableObject TrySelectLockObj(Vector3 currentPosision, Vector3 currentViewDir)
    {
        var collidersInRange = Physics.OverlapSphere(lockDistanceRef.position, lockDistanceRef.localScale.x, layerMask);
        LockableObject closestLockableObject = null;
        double closestDotProd = -1;
        //Debug.LogWarning("TrySelectLockObj colliders found "+ collidersInRange.Length);
        foreach (var colliderInRange in collidersInRange)
        {
            LockableObject lockableObject = null;
            bool isLockable = false;

            //In case it has a rigidbody check the component there
            if (colliderInRange.attachedRigidbody != null)
                isLockable = colliderInRange.attachedRigidbody.TryGetComponent<LockableObject>(out lockableObject);

            //In case no component on rigidbody or no rigidbody check component in the collider's gObj itself
            if (!isLockable)
                isLockable = colliderInRange.TryGetComponent<LockableObject>(out lockableObject);

            if (!isLockable) continue;

            var objViewDirection = (lockableObject.transform.position - currentPosision).normalized;
            var objDotProd = Vector3.Dot(currentViewDir, objViewDirection);
            if (objDotProd > closestDotProd)
            {
                closestLockableObject = lockableObject;
                closestDotProd = objDotProd;
            }
        }
        //Debug.LogWarning("closestDotProd "+ closestDotProd);
        //Debug.LogWarning("closestLockableObject "+ closestLockableObject);

        if(closestDotProd<minimumLockThreshold) closestLockableObject=null;

        return closestLockableObject;
    }


}
