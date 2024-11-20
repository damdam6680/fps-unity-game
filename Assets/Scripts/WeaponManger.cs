using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManger : MonoBehaviour
{
    public static WeaponManger Instance { get; set; }

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];
    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }
    }
    public void PickupWeapon(GameObject pikedupweapon)
    {
        AddWeaponIntoActiveSlot(pikedupweapon);
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupweapon)
    {

        DropCurrentWeapon(pickedupweapon);

        pickedupweapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedupweapon.GetComponent<Weapon>();

        pickedupweapon.transform.localPosition = new Vector3(weapon.spawnPostion.x, weapon.spawnPostion.y, weapon.spawnPostion.z);
        pickedupweapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    private void DropCurrentWeapon(GameObject pickedupweapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;

            weaponToDrop.transform.SetParent(pickedupweapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedupweapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedupweapon.transform.localRotation;

        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon curremtWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            curremtWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }
    }
}

