using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerWeaponsController : MonoBehaviour
{

    [Serializable]
    public struct WeaponEntry
    {
        public GameObject unequipedObject;
        public GameObject equipedObject;
        public RuntimeAnimatorController animator;
    }
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private List<WeaponEntry> weaponsInSlot;
    WeaponEntry? weaponInHand;

    void Start()
    {
        inputManager.input_equipWeapon.Onpressed += EquipWeapon;
    }
    private void OnDestroy()
    {
        inputManager.input_equipWeapon.Onpressed -= EquipWeapon;
    }

    int GetNextWeaponIndex()
    {
        if (!weaponInHand.HasValue) return 0;
        var currentIndex = weaponsInSlot.IndexOf(weaponInHand.Value);
        return currentIndex >= weaponsInSlot.Count - 1 ? 0 : currentIndex + 1;
    }


    public void EquipWeapon()
    {
        if (weaponInHand.HasValue)
        {
            weaponInHand.Value.equipedObject.SetActive(false);
            weaponInHand.Value.unequipedObject.SetActive(true);
            weaponInHand=null;
            animationController.SwitchAnimator(null);
        }
        else
        {
            weaponInHand = weaponsInSlot[0];
            weaponInHand.Value.equipedObject.SetActive(true);
            weaponInHand.Value.unequipedObject.SetActive(false);
            animationController.SwitchAnimator(weaponInHand.Value.animator);
        }
        animationController.Animate_EquipWeapon();
    }

    void Update()
    {

    }


    void SelectWeapon()
    {

    }
}
