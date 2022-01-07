using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;
using Uevents;

public class Gump : MonoBehaviour
{
    public float rotatioLerp = 5;
    public float moveSpeed = 10;
    public float hitForce = 25;
    public int deathDelayMs = 500;
    public int destroyDelayMs = 500;
    public VisualEffect deathParticles;
    public SkinnedMeshRenderer bodyRenderer;
    public SkinnedMeshRenderer maskRenderer;
    public MeshRenderer mask2Renderer;
    public static Uevent OnGumpDied= new Uevent();

    private UeventHandler eventHandler = new UeventHandler();

    PlayerDetection playerDetection;
    Rigidbody body;
    Animator animator;
    CapsuleCollider collider;
    public Transform playerRef;

    public bool isPlayerClose;
    public void PlayerNearby(bool isNear) => isPlayerClose = isNear;


    void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider>();
        playerRef = FindObjectOfType<PlayerAnimationController>(includeInactive: true).transform;
        playerDetection = GetComponentInChildren<PlayerDetection>(includeInactive: true);
        playerDetection.OnPlayerNearby.Subscribe(eventHandler, (isNear) => isPlayerClose = isNear);
        //playerDetection.OnPlayerNearby += (isNear) => isPlayerClose = isNear;
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }


    public async void Hit(Vector3 sourcePos)
    {
        body.AddForce(-Vector3.ProjectOnPlane((sourcePos - transform.position).normalized * hitForce, Vector3.up), ForceMode.Impulse);
        animator.SetTrigger("Death");
        maskRenderer.enabled = false;
        mask2Renderer.enabled = true;
        await Task.Delay(deathDelayMs);
        body.isKinematic = true;
        bodyRenderer.enabled = false;
        mask2Renderer.enabled = false;
        collider.enabled = false;
        deathParticles.SendEvent("OnDeath");
        await Task.Delay(destroyDelayMs);
        Destroy(gameObject);
        OnGumpDied.TryInvoke();
    }

    private void FixedUpdate()
    {
        animator.SetFloat("Speed", body.velocity.magnitude);
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
