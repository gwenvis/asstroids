using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform bulletExit;

    [Header("Weapon properties")]
    public string weaponName;
    public int maxBulletsInClip;
    public int maxBullets;
    [Tooltip("Shot/second")]
    public float fireRate;
    public float reloadTime;

    private int currentBullets;

    public bool CanShoot { get; set; }
    public float LastFireTime { get; set; }
    private float LastReloadTime { get; set; }

    public void FirstSelect()
    {
        currentBullets = maxBulletsInClip;
    }

    public void Shoot(GameObject shooter)
    {

    }
}
