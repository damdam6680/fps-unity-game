using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChanel;
    public AudioSource relodingSound1911;
    public AudioSource emptySound1911;


    public AudioSource relodingSoundAK47;

    public AudioClip ak47shoot;
    public AudioClip pistol1911shoot;

    public AudioSource throwablesChannel;
    public AudioClip grenadeSound;

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

    public void PlayShootingSound(WeaponModel weaponModel)
    {
        switch (weaponModel) 
        {
            case WeaponModel.Pistol1911:
                ShootingChanel.PlayOneShot(pistol1911shoot);
                break;
            case WeaponModel.AK47:
                ShootingChanel.PlayOneShot(ak47shoot);
                break;
        }
    }

    public void PlayRealoadSound(WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case WeaponModel.Pistol1911:
                relodingSound1911.Play();
                break;
            case WeaponModel.AK47:
                relodingSoundAK47.Play();
                break;
        }
    }

}
