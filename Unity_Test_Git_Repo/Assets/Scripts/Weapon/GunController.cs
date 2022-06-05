using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    [SerializeField] private Transform weaponHold;
    [SerializeField] private Gun startingGun;
    [SerializeField] private Gun[] playerWeapons;

    public Gun equippedGun;
    int equippedGunIndex;

    public float GunHeight
    {
        get { return weaponHold.position.y; }
    }
    private void Start()
    {
        if (playerWeapons.Length > 0 )
        {
            EquipGun(playerWeapons[0]);
            equippedGunIndex = 0;
        }
    }

    public void EquipGun(int weaponIndex)
    {
        if (equippedGunIndex == weaponIndex)
        {
            return;
        }
        equippedGunIndex = weaponIndex;
        EquipGun(playerWeapons[weaponIndex]);
    }

    private void EquipGun(Gun gunToEquip)
    {
        if(equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        if (gunToEquip != equippedGun)
        {
            equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
            equippedGun.transform.parent = weaponHold;
        }
    }

    public void OnTriggerHold()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }


    public void Shoot()
    {
        if(equippedGun != null)
        {
            equippedGun.Shoot();
        }
    }

    public void Aim(Vector3 aimPoint)
    {
        if (equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }
}
