using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class WeaponManger : MonoBehaviour
{
    public static WeaponManger Instance { get; set; }

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwables")]
    public float throwForce = 40f;
    public GameObject grenadePrefab;
    public GameObject throwableSpawn;
    public float forceMultipiler = 0;
    public float forceMultiplierLimit = 2f;
    public int lethalsCount = 0;
    public Throwable.ThrowableType equippedLethalType;

    [Header("Tacticals")]
    public int tacticalsCount = 0;
    public Throwable.ThrowableType equippedTacticalType;
    public GameObject smokeGrandePrefab;

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

        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;
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

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (lethalsCount > 0)
            {
                ThrowLethal();
            }
            forceMultipiler = 0;
        }

        if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.T))
        {
            forceMultipiler += Time.deltaTime;

            if ( forceMultipiler > forceMultiplierLimit)
            {
                forceMultipiler = forceMultiplierLimit;
            }
        }


        if (Input.GetKeyUp(KeyCode.T))
        {
            if (tacticalsCount > 0)
            {
                ThrowTactical();
            }
            forceMultipiler = 0;
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

    internal void PickupAmmo(AmmoBox ammoBox)
    {
        switch (ammoBox.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammoBox.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammoBox.ammoAmount;
                break;
        }
    }

    internal void DecreseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.AK47:
                totalRifleAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Pistol1911:
                totalPistolAmmo -= bulletsToDecrease;
                break;
        }
    }
    public int CheckAmmoLeftFOr(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.AK47:
                return totalRifleAmmo;
            case Weapon.WeaponModel.Pistol1911:
                return totalPistolAmmo;
            default:
                return 0;
        }
    }

    #region || -- Throwables -- ||
    public void PickupThrowable(Throwable throwable)
    {
        switch(throwable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                // PickupGrenade();
                PickupThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Smoke_Granade:
                // PickupGrenade();
                PickupThrowableAsTactical(Throwable.ThrowableType.Smoke_Granade);
                break;
        }
    }

    private void PickupThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        if (equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;

            if (tacticalsCount < 2)
            {
                tacticalsCount += 1;
                Destroy(InteractionManger.Instance.hoverThrowable.gameObject);
                HudManger.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("TacticalsCount limit reached");
            }
        }
        else
        {

        }
    }

    private void PickupThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if (lethalsCount < 2)
            {
                lethalsCount += 1;
                Destroy(InteractionManger.Instance.hoverThrowable.gameObject);
                HudManger.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Lethals limit reached");
            }
        }
        else
        {

        }
    }


    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);

        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce  * forceMultipiler), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        lethalsCount -= 1;

        if (lethalsCount <= 0) 
        {
            equippedLethalType = Throwable.ThrowableType.None;
        }
        HudManger.Instance.UpdateThrowablesUI();
    }

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);

        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultipiler), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        tacticalsCount -= 1;

        if (tacticalsCount <= 0)
        {
            equippedTacticalType = Throwable.ThrowableType.None;
        }
        HudManger.Instance.UpdateThrowablesUI();
    }


    private GameObject GetThrowablePrefab(Throwable.ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Smoke_Granade:
                return smokeGrandePrefab;
        }

        return new();
    }

    #endregion
}

