using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gump : MonoBehaviour
{
    public float rotatioLerp = 5;
    public float moveSpeed = 10;
    public float hitForce = 25;

    PlayerDetection playerDetection;
    Rigidbody body;
    Animator animator;
    public Transform playerRef;
    public bool isPlayerClose;
    public void PlayerNearby(bool isNear) => isPlayerClose = isNear;


    void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerRef = FindObjectOfType<PlayerAnimationController>(includeInactive: true).transform;
        playerDetection = GetComponentInChildren<PlayerDetection>(includeInactive: true);
        playerDetection.OnPlayerNearby += (isNear) => isPlayerClose = isNear;
    }


    public void Hit(Vector3 sourcePos)
    {
        body.AddForce(-Vector3.ProjectOnPlane((sourcePos - transform.position).normalized * hitForce,Vector3.up),ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        animator.SetFloat("Speed",body.velocity.magnitude);
        if (isPlayerClose)
        {
            transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane((playerRef.position - transform.position).normalized, Vector3.up), Time.fixedDeltaTime * rotatioLerp);
            body.AddForce(transform.forward * body.mass * moveSpeed);
        }
        else
        {
        }
    }

}
