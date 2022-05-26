using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : MonoBehaviour
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float timeBetweenShots = 100; // in milisecunde
    [SerializeField] private float muzzleVelocity = 30;
    [SerializeField] private float nextShotTime = 1;


    public void Shoot()
    {
        if(Time.time > nextShotTime)
        {
            nextShotTime = Time.time + timeBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile; 
            newProjectile.SetSpeed(muzzleVelocity);
        }
    }

}
