using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UEventHandler;

//public delegate void AttackAction(int lvl, bool isRestoringCombo);

public class AttackController : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private int ComboSize;
    [SerializeField] public float ComboRestoreTime;
    [SerializeField] public bool canRestoreCombo;

    public  UEvent<int,bool> OnAttack= new UEvent<int, bool>();
    public int ComboIndex { get; private set; }

    public bool IsAttacking { get; set; }

    public void ResetComboIndex() => ComboIndex = 0;
    public void IncrementComboIndex() => ComboIndex = ComboIndex >= ComboSize ? 1 : ComboIndex + 1;

    UEventHandler eventHandler = new UEventHandler();

    // [HideInInspector]
    [Header("Exposed Variables")]
    public bool canAttack = true;
    public bool restoreAttackCombo;
    public bool canIncrementAttack;
    public bool isAttacking;

    Coroutine resetAttackComboRoutine;

    void Start()
    {
        canAttack = true;
        canIncrementAttack = true;

        inputManager.input_attack.Onpressed.Subscribe(eventHandler, Attack);
        PauseHandler.OnPause.Subscribe(eventHandler, () => canAttack = false);
        PauseHandler.OnUnpause.Subscribe(eventHandler, () => canAttack = true);
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();

    }

    public void NewAttackStarted()
    {
        canIncrementAttack = true;
    }
    public void ComboEnded()
    {
        canIncrementAttack = true;
        isAttacking = false;
        movementController.LockMovement(false);
        movementController.LockTurning(false);
        if (resetAttackComboRoutine != null) StopCoroutine(resetAttackComboRoutine);

        if (canRestoreCombo)
            resetAttackComboRoutine = StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        restoreAttackCombo = true;
        yield return new WaitForSeconds(ComboRestoreTime);
        //Debug.LogError("ResetAttack -> canRestoreAttackCombo");
        restoreAttackCombo = false;
    }

    void Attack()
    {
        if (!canAttack) return;

        //Debug.LogError("----------------Attack");
        if (!canIncrementAttack)
        {
            //Debug.LogError("canIncrementAttack false");
            return;
        }
        canIncrementAttack = false;
        if (!isAttacking && !restoreAttackCombo)
        {

            //Debug.LogError("ResetComboIndex");
            ResetComboIndex();
        }
        //Debug.LogError("IncrementComboIndex");
        IncrementComboIndex();

        //Debug.LogError("canRestoreAttackCombo animator var -> " + (!isAttacking && ComboIndex > 1 && canRestoreAttackCombo));

        movementController.LockMovement();
        movementController.LockTurning();
        OnAttack.TryInvoke(ComboIndex, !isAttacking && ComboIndex > 1 && restoreAttackCombo);
        isAttacking = true;
    }



    // Update is called once per frame
    void Update()
    {

    }
}
