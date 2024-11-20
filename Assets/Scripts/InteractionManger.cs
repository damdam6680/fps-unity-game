using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionManger : MonoBehaviour
{
    public static InteractionManger Instance { get; set; }

    public Weapon hoverweapon = null;

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

        // Flaga czy obiekt zosta� trafiony
        bool weaponHit = false;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;

            if (objectHitByRaycast.GetComponent<Weapon>() && objectHitByRaycast.GetComponent<Weapon>().isActiveWeapon == false)
            {
                Weapon detectedWeapon = objectHitByRaycast.GetComponent<Weapon>();

                // Je�li trafiamy w now� bro�, wy��cz obrys poprzedniej
                if (hoverweapon != null && hoverweapon != detectedWeapon)
                {
                    hoverweapon.GetComponent<Outline>().enabled = false;
                }

                // Zaznacz now� bro�
                hoverweapon = detectedWeapon;
                hoverweapon.GetComponent<Outline>().enabled = true;
                Debug.Log(hoverweapon + " true");

                weaponHit = true;

                // Obs�uga podniesienia broni
                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManger.Instance.PickupWeapon(objectHitByRaycast);
                    hoverweapon.GetComponent<Outline>().enabled = false;
                    hoverweapon = null; // Zresetuj hoverweapon po podniesieniu
                }
            }
        }

        // Je�li promie� nie trafia w �aden obiekt broni
        if (!weaponHit && hoverweapon != null)
        {
            hoverweapon.GetComponent<Outline>().enabled = false;
            Debug.Log(hoverweapon + " false");
            hoverweapon = null; // Zresetuj hoverweapon
        }
    }
}

