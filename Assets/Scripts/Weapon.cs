using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    //Strzelanie
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 0.5f;

    //Burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    //Spred
    [Header("Spread")]
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    //Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    //Anim
    public GameObject muzzleEffect;
    internal Animator animator;

    //Reload
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloding;

    //pos on fp
    public Vector3 spawnPostion;
    public Vector3 spawnRotation;

    bool isADS;
    public enum WeaponModel
    {
        Pistol1911,
        AK47,
    }

    public WeaponModel thisWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;


    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveWeapon)
        {
            if (Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }
            if (Input.GetMouseButtonUp(1)) 
            {
                ExitADS();
            }

            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptySound1911.Play();
            }

            if (currentShootingMode == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);

            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                isShooting = (Input.GetKeyDown(KeyCode.Mouse0));
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloding == false && WeaponManger.Instance.CheckAmmoLeftFOr(thisWeaponModel) > 0)
            {
                Reload();
            }
        }
    }

    private void EnterADS() 
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HudManger.Instance.MiddleDoot.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }

    private void ExitADS() 
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        HudManger.Instance.MiddleDoot.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        if (isADS)
        {
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {
            animator.SetTrigger("RECOIL");
        }
        

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread();

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        bullet.transform.forward = shootingDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShoot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }

    }

    private void Reload()
    {
        SoundManager.Instance.PlayRealoadSound(thisWeaponModel);
        if (isADS)
        {
            // reload ads
        }
        else
        {
          animator.SetTrigger("RELOAD");
        }
        

        isReloding = true;
        readyToShoot = false;
        Invoke("ReloadComplited", reloadTime);
    }

    private void ReloadComplited()
    {
        if (WeaponManger.Instance.CheckAmmoLeftFOr(thisWeaponModel) > magazineSize)
        {
            bulletsLeft = magazineSize;
            WeaponManger.Instance.DecreseTotalAmmo(bulletsLeft, thisWeaponModel);
        }
        else
        {
            bulletsLeft = WeaponManger.Instance.CheckAmmoLeftFOr(thisWeaponModel);
            WeaponManger.Instance.DecreseTotalAmmo(bulletsLeft, thisWeaponModel);
        }
        isReloding = false;
        readyToShoot = true;
    }

    private void ResetShoot()
    {
        readyToShoot = true;
        allowReset = true;
    }


    private Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100); 
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        // Add spread
        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        Debug.Log($"Direction: {direction}, Spread X: {z}, Spread Y: {y}");
        direction += new Vector3(0, y, z);
        return direction.normalized;
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPrefabLifeTime)
    {
        yield return new WaitForSeconds(bulletPrefabLifeTime);
        Destroy(bullet);
    }
}
