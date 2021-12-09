using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public delegate void MovementAction();


[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("External references")]
    public InputManager inputManager;
    public AimController aimController;
    public Transform playerMesh;

    [Header("Basic Movement Walk/Sprint")]
    public float walkSpeed = 5;
    public float maxWalkSpeed = 15;
    public float minGroundDistace=.1f;
    public AnimationCurve movementSpeedCurve;
    public float sprintSpeedFactor = 5;
    public LayerMask layerMask;
    public float castMaxDistance=5;


    [Header("Jump")]
    public int jumpBufferMs=400;
    public float maxJumpSpeed=3;
    public int jumpForceTicks=1;
    public float jumpAcceleration=7;

    [Header("Character Rotation")]
    public float lerpRotation=21;

    [Header("Freeze Movement")]
    public float freezeDecelFactor = 0.05f;

    [Header("Exposed Values")]
    public Vector3 groundNormal;
    public float groundDistance;



    //Inspector Hidden parameters
    public Vector3 playerHorizontalDirection { get; private set; }
    public Vector3 upLeanedVector { get; private set; }
    public Vector3 playerHorizontalVelocity { get; private set; }
    public bool isInLockState { get; private set; }
    public event MovementAction onJumped;
    public event MovementAction onLanded;
    public event MovementAction onFalling;


    CharacterController characterController;
    Vector3 acceleration = Vector3.zero;
    Vector3 dampedAcceleration = Vector3.zero;
    Vector3 lastVelocity = Vector3.zero;
    Vector3 playerSpeed = Vector3.zero;
    Vector3 playerStartingPos;

    bool isSprinting, isCheckingLand, isTurningLocked, isMovementLocked, jumpBuffer, isFrozen;
    int jumpTickCounter = 0;

    public void LockTurning(bool locked = true) => isTurningLocked = locked;
    public void LockMovement(bool locked = true) => isMovementLocked = locked;


    private void Awake()
    {
        playerStartingPos=transform.position;
    }

    void Start()
    {
        //Time.timeScale = .5f;
        groundNormal = Vector3.up;
        inputManager.input_jump.Onpressed += StartJumpBuffer;
        characterController = GetComponent<CharacterController>();
        Chest.OnChestOpened += () => isFrozen = true;
        Chest.OnChestItemShow += (x,y,z) => isFrozen = false;
        aimController.OnLockTarget += () => isInLockState = true;
        aimController.OnUnlockTarget += () => isInLockState = false;
        PlayerCutsceneManager.OnIntroStarted+=  () => isFrozen = true;
        PlayerCutsceneManager.OnIntroFinished+=  () => isFrozen = false;
        UIManager.OnStartedDialogue+=  (x,y) => isFrozen = true;
        UIManager.OnFinishedDialogue+=  () => isFrozen = false;
        GameManager.OnPlayerReset += ResetPlayerPos;

        if (PlayerCutsceneManager.isIntroEnabled) isFrozen = true;   
    }

    private void OnDestroy()
    {
        inputManager.input_jump.Onpressed -= StartJumpBuffer;
        aimController.OnLockTarget -= () => isInLockState = true;
        aimController.OnUnlockTarget -= () => isInLockState = false;
        Chest.OnChestOpened -= () => isFrozen = true;
        Chest.OnChestItemShow -= (x, y, z) => isFrozen = false;
        PlayerCutsceneManager.OnIntroStarted-=  () => isFrozen = true;
        PlayerCutsceneManager.OnIntroFinished -= () => isFrozen = false;
        GameManager.OnPlayerReset -= ResetPlayerPos;
        UIManager.OnStartedDialogue -= (x, y) => isFrozen = true;
        UIManager.OnFinishedDialogue -= () => isFrozen = false;
    }

    void Update()
    {
        if (isFrozen) return;

        GetGroundNormal();
        CalculateDirection();
        CheckIfJumpLanded();
        InitiateJump();

        playerHorizontalVelocity = Vector3.Scale(characterController.velocity, new Vector3(1, 0, 1));

        if (!isTurningLocked)
        {
            if (!isInLockState) StandardTurning();
            else LockViewTurning();
        }
        Move(ref playerSpeed);
        HandleGravity(ref playerSpeed);
        JumpForce(ref playerSpeed);

        characterController.Move(playerSpeed * Time.deltaTime);
    }

    void GetGroundNormal()
    {
        //if (!characterController.isGrounded) return;

        RaycastHit hit;
        var hasHit= Physics.Raycast(transform.position, Vector3.down,out hit, castMaxDistance, layerMask);
        //Debug.DrawRay(transform.position, Vector3.down, Color.red);
        if (!hasHit) return;
        Debug.DrawRay(hit.point, hit.normal,Color.blue);
        groundDistance= hit.distance;
        groundNormal = hit.normal;
    }

    void CalculateDirection()
    {
        var frontalDirection = aimController.aimTarget.forward.normalized * inputManager.input_move.value.y;
        var sideDirection = aimController.aimTarget.right.normalized * inputManager.input_move.value.x;
        playerHorizontalDirection = Vector3.Scale((frontalDirection + sideDirection), new Vector3(1, 0, 1)).normalized;
    }

    void HandleGravity(ref Vector3 currentVelocity)
    {

        currentVelocity.y += Physics.gravity.y * Time.deltaTime;
        
        if (characterController.isGrounded)
            currentVelocity.y = -.05f;
    }

    void JumpForce(ref Vector3 currentSpeed)
    {
        if (jumpTickCounter <= 0) return;

        jumpTickCounter--;
        currentSpeed += Vector3.up * jumpAcceleration;
    }




    void StandardTurning()
    {
        if (inputManager.input_move.value.y == 0 && inputManager.input_move.value.x == 0) return;

        var calculatedForwardVector = Vector3.Lerp(playerMesh.forward, playerHorizontalDirection.normalized, Time.deltaTime * lerpRotation);
        playerMesh.rotation = Quaternion.Lerp(playerMesh.rotation, Quaternion.LookRotation(playerHorizontalDirection.normalized), Time.deltaTime * lerpRotation);
    }

    void LockViewTurning()
    {
        playerMesh.forward = Vector3.Lerp(playerMesh.forward, Vector3.Scale(aimController.aimTarget.forward, new Vector3(1, 0, 1)), Time.deltaTime * lerpRotation); ;
    }


    void Move(ref Vector3 currentSpeed)
    {
        //if (!characterController.isGrounded) return;

        if (groundDistance > minGroundDistace) return;

        if (isMovementLocked)
        {
            bool xSpeedSign = currentSpeed.x > 0;
            bool zSpeedSign = currentSpeed.z > 0;

            var xDecelValue = xSpeedSign ? -1 : 1;
            var zDecelValue = zSpeedSign ? -1 : 1;

            //Case speed already zero no decel value
            if (currentSpeed.x == 0) xDecelValue = 0;
            if (currentSpeed.z == 0) zDecelValue = 0;

            currentSpeed += new Vector3(xDecelValue, 0, zDecelValue) * freezeDecelFactor;
            //currentSpeed += Vector3.ProjectOnPlane(new Vector3(xDecelValue, 0, zDecelValue) * freezeDecelFactor,);

            //In case decel inverted speed bring it to zero
            if (xSpeedSign && currentSpeed.x < 0) currentSpeed.x = 0;
            if (zSpeedSign && currentSpeed.z < 0) currentSpeed.z = 0;
            return;
        }

        var horziontalDirection = !isInLockState ? playerMesh.forward.normalized : playerHorizontalDirection;

        var maxSpeed = inputManager.input_sprint.value > 0 ? maxWalkSpeed * sprintSpeedFactor : maxWalkSpeed;
        var maxWalkForce = inputManager.input_sprint.value > 0 ? walkSpeed * sprintSpeedFactor : walkSpeed;

        float speedFactor = walkSpeed;
        var clampedSpeed = Vector3.Scale(characterController.velocity, new Vector3(1, 0, 1)).magnitude;
        if (clampedSpeed > maxSpeed)
            clampedSpeed = maxSpeed;


        speedFactor = movementSpeedCurve.Evaluate(clampedSpeed / maxSpeed) * maxWalkForce;

        var inputFactor = inputManager.input_move.value.normalized.magnitude;

        var characterSpeed = playerHorizontalDirection * speedFactor * inputFactor;

        var tmpYSpeed = currentSpeed.y;

        currentSpeed = Vector3.ProjectOnPlane(new Vector3(characterSpeed.x, currentSpeed.y, characterSpeed.z),groundNormal);
      
        //currentSpeed.y = !characterController.isGrounded && currentSpeed.y < 0 ? currentSpeed.y + tmpYSpeed : currentSpeed.y;

        Debug.DrawRay(transform.position, currentSpeed, Color.red);
    }

    async void StartJumpBuffer()
    {
        jumpBuffer = true;
        await Task.Delay(jumpBufferMs);
        jumpBuffer = false;
    }

    async void InitiateJump()
    {
        if (!jumpBuffer || !characterController.isGrounded) return;


        jumpBuffer = false;
        onJumped.Invoke();
        LockMovement();
        LockTurning();
        jumpTickCounter = jumpForceTicks;

        await Task.Delay(100);
        isCheckingLand = true;
    }


    void CheckIfJumpLanded()
    {
        if (!isCheckingLand || !characterController.isGrounded) return;

        isCheckingLand = false;
        LockMovement(false);
        LockTurning(false);
        onLanded.Invoke();
    }

    private async void ResetPlayerPos()
    {
        await Task.Delay(GameManager.currentGameManager.resetFreezeDurationMs);
        transform.position = playerStartingPos;
    }
}
