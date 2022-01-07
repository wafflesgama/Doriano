using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Uevents;

public class PlayerDamageHandler : MonoBehaviour
{
    public delegate void HitAction(Vector3 source);
    public static Uevent<Vector3> OnHit = new Uevent<Vector3>();

    public AttackController attackController;
    public Collider damageZoneTrigger;
    public int activationDelayMs = 300;

    UeventHandler eventHandler = new UeventHandler();
    void Start()
    {
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player") return;

    //    var point = other.ClosestPoint(transform.position);
    //    OnHit.TryInvoke(point);

    //    if (other.tag == "Enemy")
    //    {
    //        other.gameObject.GetComponent<Gump>().Hit(attackController.transform.position);
    //    }
    //}
}
