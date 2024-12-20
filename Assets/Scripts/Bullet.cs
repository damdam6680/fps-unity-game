using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision objectWeHit)
    {

        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            print("Hit Target: " + objectWeHit.gameObject.name);

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            print("Hit Wall");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Bottle"))
        {
            print("Hit Bottle");

            objectWeHit.gameObject.GetComponent<Bottle_>().Shatter();
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(
             GlobalReferences.Instance.bulletImpactEffectPrefb,
             contact.point,
             Quaternion.LookRotation(contact.normal)
            ) ;

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
