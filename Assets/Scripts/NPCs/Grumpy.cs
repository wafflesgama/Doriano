using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Uevents;

public class Grumpy : MonoBehaviour, Interactable
{
    [Header("Dialogues")]
    [Multiline] public string[] initDialogue;
    [Multiline] public string[] doneDialogue;
    [Multiline] public string[] afterDoneDialogue;

    [Header("Animators param names")]
    public string animatorBlockName;
    public string animatorSpeedName;
    public string animatorAnnoyedName;

    [Header("Other params")]
    public float numGumpsNecessary;
    public Vector3 messageOffset;
    public float characterHeightOffset;
    public float minZ, maxZ, minX, maxX, lerpSpeed;
    public float maxDistanceMultp;
    public AnimationCurve lerpMultiplier;
    public float xSpeed;

    public Collider wallRef;

    Animator animator;
    bool isBlockingPath, hasUnlockedPath;
    Transform playerRef;
    Plane wallPlane;

    UeventHandler eventHandler = new UeventHandler();
    public Vector3 GetOffset() => messageOffset;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        wallPlane = new Plane(wallRef.transform.forward, wallRef.transform.position);
    }

    void Start()
    {
        UIManager.OnFinishedDialogue.Subscribe(eventHandler, FinishedDialogue);
    }

    public void Interact()
    {
        if (!hasUnlockedPath)
        {
            var result = CheckIfUnlocked();
            if (!result)
                UIManager.OnStartedDialogue.TryInvoke(transform, initDialogue);
            else
            {
                hasUnlockedPath = true;
                animator.SetTrigger(animatorAnnoyedName);
                wallRef.gameObject.SetActive(false);
                UIManager.OnStartedDialogue.TryInvoke(transform, doneDialogue);
            }

        }
        else
        {
            UIManager.OnStartedDialogue.TryInvoke(transform, afterDoneDialogue);
        }
        gameObject.tag = "Uninteractable";
    }

    private bool CheckIfUnlocked() => hasUnlockedPath = GameManager.currentGameManager.gumpKilled >= numGumpsNecessary;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !other.isTrigger)
        {
            playerRef = other.transform;
            isBlockingPath = true;
            animator.SetBool(animatorBlockName, isBlockingPath);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !other.isTrigger)
        {
            isBlockingPath = false;
            animator.SetBool(animatorBlockName, isBlockingPath);
        }
    }

    private void Update()
    {
        if (isBlockingPath && !hasUnlockedPath)
        {
            //var targetPos=wallRef.ClosestPoint(playerRef.position);
            var distance = Vector3.Distance(wallRef.transform.position, playerRef.transform.position);
            distance = Mathf.Clamp(distance, 0, maxDistanceMultp);
            var factor = lerpMultiplier.Evaluate(distance / maxDistanceMultp);
            var targetPos = wallPlane.ClosestPointOnPlane(playerRef.position);
            var lastPos = transform.position;
            transform.position = Vector3.Lerp(transform.position, new Vector3(targetPos.x, transform.position.y + characterHeightOffset, targetPos.z), Time.deltaTime * lerpSpeed * factor);
            var speedIsRight= Vector3.Dot(transform.position - lastPos,transform.right);
            xSpeed= Vector3.Distance(transform.position, lastPos)/Time.deltaTime;
            xSpeed*=speedIsRight> 0?1:-1;
            animator.SetFloat(animatorSpeedName, xSpeed);
            //animator.SetBool(animatorBlockVarName, isBlockingPath);
        }
    }

    private async void FinishedDialogue()
    {
        await Task.Delay(1000);
        gameObject.tag = "Interactable";
    }
}
