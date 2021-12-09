using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    public AttackController attackController;
    public Collider damageZoneTrigger;
    public int triggerLockCounter;

    void Start()
    {
        damageZoneTrigger.enabled = false;
        triggerLockCounter = 0;
        attackController.OnAttack += AttackController_OnAttack;
    }

    private async void AttackController_OnAttack(int lvl, bool isRestoringCombo)
    {

        await Task.Delay(300);
        triggerLockCounter++;
        damageZoneTrigger.enabled = true;
        await Task.Delay(300);
        triggerLockCounter--;
        if (triggerLockCounter < 0) triggerLockCounter = 0;

        if (triggerLockCounter == 0)
            damageZoneTrigger.enabled = false;


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag== "Enemy")
        {
            other.gameObject.GetComponent<Gump>().Hit(attackController.transform.position);
        }
    }
}
