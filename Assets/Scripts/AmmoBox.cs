using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 200;
    public AmmoType ammoType;
    public bool isAnimated = false;

    // Parametry animacji
    public float floatAmplitude = 0.5f; // Amplituda unoszenia
    public float floatSpeed = 1f; // Prêdkoœæ unoszenia
    public float rotationSpeed = 50f; // Prêdkoœæ rotacji

    private Vector3 startPosition;

    public enum AmmoType
    {
        RifleAmmo,
        PistolAmmo
    }



    private void Start()
    {

        startPosition = transform.position;
    }

    private void Update()
    {
        if (isAnimated) { 
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        }
    }
}

