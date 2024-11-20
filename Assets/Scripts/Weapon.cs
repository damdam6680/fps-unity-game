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
    public float speadIntensity;

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


  
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveWeapon)
        {
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

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloding == false)
            {
                Reload();
            }
            if (AmmoManager.Instance.ammoDisplay != null)
            {
                AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}";
            }
        }
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

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

        //Update ui
        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
        }
    }

    private void Reload()
    {
        SoundManager.Instance.PlayRealoadSound(thisWeaponModel);
        animator.SetTrigger("RELOAD");

        isReloding = true;
        readyToShoot = false;
        Invoke("ReloadComplited", reloadTime);
    }

    private void ReloadComplited()
    {
        bulletsLeft = magazineSize;
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
        float x = UnityEngine.Random.Range(-speadIntensity, speadIntensity);
        float y = UnityEngine.Random.Range(-speadIntensity, speadIntensity);
        Debug.Log($"Direction: {direction}, Spread X: {x}, Spread Y: {y}");
        direction += new Vector3(x, y, 0);
        return direction.normalized;
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPrefabLifeTime)
    {
        yield return new WaitForSeconds(bulletPrefabLifeTime);
        Destroy(bullet);
    }
}
