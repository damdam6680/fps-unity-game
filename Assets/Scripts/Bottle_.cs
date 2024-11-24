using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle_ : MonoBehaviour
{
    [SerializeField] GameObject brokenBottlePrefab;

        void Update() // just for testing
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Shatter();
        }
    }
    public void Shatter()
    {
        GameObject brokenBottle = Instantiate(brokenBottlePrefab, this.transform.position, Quaternion.identity);
        brokenBottle.GetComponent<BrokenBottle>().RandomVelocities();
        Destroy(gameObject);
    }
}
