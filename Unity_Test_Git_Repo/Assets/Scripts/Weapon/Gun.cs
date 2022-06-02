using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : MonoBehaviour
{
    [SerializeField] private int projectilePerMag;
    [SerializeField] private float reloadTime = .3f;

    [Header("Effects")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float timeBetweenShots = 100; // in milisecunde
    [SerializeField] private float muzzleVelocity = 30;
    [SerializeField] private float nextShotTime = 1;

    [Header("Recoil")]
    [SerializeField] private Vector2 kickMinMax = new Vector2(.05f, .2f);
    [SerializeField] private Vector2 recoilAngleMinMax = new Vector2(10,20);
    [SerializeField] private float recoilMoveSettleTime = .1f;
    [SerializeField] private float recoilRotationSettleTime = .1f;


    public Transform shell;
    public Transform shellEjection;
    private Vector3 recoilSmoothDampVelocity;
    private float recoilAngle;
    private float recoilRotSmoothDampVelocity;
    private bool isReloading;

    int projectileRemainingInMag;


    MuzzleFlash muzzleFlash;
    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        projectileRemainingInMag = projectilePerMag;
    }

    private void LateUpdate()
    {
        // animate recoil

        //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        //recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        //transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!isReloading && projectileRemainingInMag == 0)
        {
            Reload();
        }
    }

    public void Shoot()
    {
        if(!isReloading && Time.time > nextShotTime && projectileRemainingInMag > 0)
        {
            if (projectileRemainingInMag == 0)
            {
                return;
            }
            projectileRemainingInMag--;
            nextShotTime = Time.time + timeBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile; 
            newProjectile.SetSpeed(muzzleVelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
        }
    }

    public void Reload()
    {
        if (!isReloading &&  projectileRemainingInMag != projectilePerMag)
        {
            StartCoroutine(AnimteReload());
        }
    }

    IEnumerator AnimteReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float percent = 0;
        float reloadSpeed = 1f / reloadTime;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30; 

        while (percent < 1)
        {
            percent += Time.deltaTime + reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }


        isReloading = false;
        projectileRemainingInMag = projectilePerMag;
    }


    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }


}
