using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    public delegate void HitAction(Vector3 source);
    public static event HitAction OnHit;

    public AttackController attackController;
    public Collider damageZoneTrigger;
    public int activationDelayMs = 300;
    //public int triggerLockCounter;

    void Start()
    {
        //damageZoneTrigger.enabled = false;
        //triggerLockCounter = 0;
        attackController.OnAttack += AttackController_OnAttack;
    }

    private async void AttackController_OnAttack(int lvl, bool isRestoringCombo)
    {

        await Task.Delay(activationDelayMs);
        DetectHit();

    }

    //private async void AttackController_OnAttack(int lvl, bool isRestoringCombo)
    //{

    //    await Task.Delay(activationDelayMs);
    //    triggerLockCounter++;
    //    damageZoneTrigger.enabled = true;
    //    await Task.Delay(activationDelayMs);
    //    triggerLockCounter--;
    //    if (triggerLockCounter < 0) triggerLockCounter = 0;

    //    if (triggerLockCounter == 0)
    //        damageZoneTrigger.enabled = false;


    //}

    private void DetectHit()
    {
        var colliders = Physics.OverlapBox(transform.position, transform.localScale);
        foreach (var collider in colliders)
        {
            if (collider.tag == "Player") continue;

            var point = collider.ClosestPoint(transform.position);
            OnHit?.Invoke(point);

            if (collider.tag == "Enemy")
                collider.gameObject.GetComponent<Gump>().Hit(attackController.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") return;

        var point = other.ClosestPoint(transform.position);
        OnHit?.Invoke(point);

        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<Gump>().Hit(attackController.transform.position);
        }
    }
}
