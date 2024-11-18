using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("Hit Target: " + collision.gameObject.name);
            Destroy(gameObject);
        }
    }
}
