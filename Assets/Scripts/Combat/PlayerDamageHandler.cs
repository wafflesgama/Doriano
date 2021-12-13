using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UEventHandler;

public class PlayerDamageHandler : MonoBehaviour
{
    public delegate void HitAction(Vector3 source);
    public static UEvent<Vector3> OnHit=new UEvent<Vector3>();

    public AttackController attackController;
    public Collider damageZoneTrigger;
    public int activationDelayMs = 300;
    //public int triggerLockCounter;

    UEventHandler eventHandler = new UEventHandler();
    void Start()
    {
        //damageZoneTrigger.enabled = false;
        //triggerLockCounter = 0;
        attackController.OnAttack.Subscribe(eventHandler, AttackController_OnAttack);
    }
    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
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
            if (collider.tag == "Player" || collider.isTrigger) continue;

            var point = collider.ClosestPoint(transform.position);
            OnHit.TryInvoke(point);

            if (collider.tag == "Enemy")
                collider.gameObject.GetComponent<Gump>().Hit(attackController.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") return;

        var point = other.ClosestPoint(transform.position);
        OnHit.TryInvoke(point);

        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<Gump>().Hit(attackController.transform.position);
        }
    }
}
