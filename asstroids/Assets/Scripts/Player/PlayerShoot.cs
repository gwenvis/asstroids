using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private Weapon currentWeapon;

    public void ChangeWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
    }

    public void Shoot()
    {
        if (currentWeapon.CanShoot)
            currentWeapon.Shoot(gameObject);
    }
}
