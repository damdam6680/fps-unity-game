using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionManger : MonoBehaviour
{
    public static InteractionManger Instance { get; set; }

    public Weapon hoverweapon = null;

    public AmmoBox hoverAmmoBox = null;

    public Throwable hoverThrowable = null;

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

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Flaga czy obiekt zosta³ trafiony
        bool weaponHit = false;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;

            if (objectHitByRaycast.GetComponent<Weapon>() && objectHitByRaycast.GetComponent<Weapon>().isActiveWeapon == false)
            {
                Weapon detectedWeapon = objectHitByRaycast.GetComponent<Weapon>();

                // Jeœli trafiamy w now¹ broñ, wy³¹cz obrys poprzedniej
                if (hoverweapon != null && hoverweapon != detectedWeapon)
                {
                    hoverweapon.GetComponent<Outline>().enabled = false;
                }

                // Zaznacz now¹ broñ
                hoverweapon = detectedWeapon;
                hoverweapon.GetComponent<Outline>().enabled = true;
                Debug.Log(hoverweapon + " true");

                weaponHit = true;

                // Obs³uga podniesienia broni
                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManger.Instance.PickupWeapon(objectHitByRaycast);
                    hoverweapon.GetComponent<Outline>().enabled = false;
                    hoverweapon = null; 
                }
            }


            if (objectHitByRaycast.GetComponent<AmmoBox>())
            {
                // Obs³uga AmmoBox
                if (hoverAmmoBox != null && hoverAmmoBox != objectHitByRaycast.GetComponent<AmmoBox>())
                {
                    // Wy³¹cz obrys poprzedniej skrzynki, jeœli hoveruje inna
                    hoverAmmoBox.GetComponent<Outline>().enabled = false;
                }

                hoverAmmoBox = objectHitByRaycast.GetComponent<AmmoBox>();
                hoverAmmoBox.GetComponent<Outline>().enabled = true;



                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManger.Instance.PickupAmmo(hoverAmmoBox);
                    hoverAmmoBox.GetComponent<Outline>().enabled = false;

                    // Przed zniszczeniem usuñ referencjê
                    AmmoBox tempAmmoBox = hoverAmmoBox;
                    hoverAmmoBox = null;

                    // Usuñ obiekt
                    Destroy(tempAmmoBox.gameObject);
                }
            }
            else
            {
                // Jeœli hoverAmmoBox istnieje i promieñ nie trafia w AmmoBox
                if (hoverAmmoBox != null)
                {
                    hoverAmmoBox.GetComponent<Outline>().enabled = false;
                    hoverAmmoBox = null;
                }
            }

            //Granat
            if (objectHitByRaycast.GetComponent<Throwable>())
            {
                // Obs³uga Granat
                if (hoverThrowable != null && hoverThrowable != objectHitByRaycast.GetComponent<Throwable>())
                {
                    
                    hoverThrowable.GetComponent<Outline>().enabled = false;
                }

                hoverThrowable = objectHitByRaycast.GetComponent<Throwable>();
                hoverThrowable.GetComponent<Outline>().enabled = true;



                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManger.Instance.PickupThrowable(hoverThrowable);
                    hoverThrowable.GetComponent<Outline>().enabled = false;

                    // Przed zniszczeniem usuñ referencjê
                    Throwable tempThrowable = hoverThrowable;
                    hoverThrowable = null;

                    // Usuñ obiekt
                    Destroy(tempThrowable.gameObject);
                }
            }
            else
            {
                // Jeœli hoverThrowable istnieje i promieñ nie trafia w AmmoBox
                if (hoverThrowable != null)
                {
                    hoverThrowable.GetComponent<Outline>().enabled = false;
                    hoverThrowable = null;
                }
            }
        }

        // Jeœli promieñ nie trafia w ¿aden obiekt broni
        if (!weaponHit && hoverweapon != null)
        {
            hoverweapon.GetComponent<Outline>().enabled = false;
            Debug.Log(hoverweapon + " false");
            hoverweapon = null; // Zresetuj hoverweapon
        }


    }
}

