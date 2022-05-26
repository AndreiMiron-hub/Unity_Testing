using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    [SerializeField] private Transform WeaponHold;
    [SerializeField] private Gun startingGun;

    Gun equippedGun;

    private void Start()
    {
        if (startingGun != null)
        {
            EquipGun(startingGun);
        }
    }

    private void EquipGun(Gun gunToEquip)
    {
        if(equippedGun != null)
        {
            Destroy(equippedGun);
        }

        equippedGun = Instantiate(gunToEquip, WeaponHold.position, WeaponHold.rotation) as Gun;
        equippedGun.transform.parent = WeaponHold;
    }

    public void Shoot()
    {
        if(equippedGun != null)
        {
            equippedGun.Shoot();
        }
    }

}
