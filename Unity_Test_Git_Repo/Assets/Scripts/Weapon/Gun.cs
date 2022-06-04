using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode;

    [SerializeField] private int projectilePerMag;
    [SerializeField] private float reloadTime = .3f;

    [Header("Effects")]
    [SerializeField] private Transform[] projectileSpawn; // de unde se spawneaza proiectilele
    [SerializeField] private Projectile projectile;
    [SerializeField] private float timeBetweenShots = 100; // in milisecunde
    [SerializeField] private float muzzleVelocity = 30;
    [SerializeField] private float nextShotTime = 1;
    [SerializeField] private int burstCount = 1;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

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

    int projectilesRemainingInMag;
    bool triggerReleasedSinceLastShot;
    bool isReloading;
    int shotsRemainingInBurst;

    MuzzleFlash muzzleFlash;
    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilePerMag;
    }

    private void LateUpdate()
    {
        // animate recoil

        //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        //recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        //transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }

    public void Shoot()
    {
        if(!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0)
        {

            switch (fireMode)
            {
                case FireMode.Burst:
                    if (shotsRemainingInBurst == 0)
                    {
                        return;
                    }
                    shotsRemainingInBurst--;
                    break;
                case FireMode.Single:
                    if (!triggerReleasedSinceLastShot)
                    {
                        return;
                    }
                    break;
                default:

                    break;
            }
            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                if (projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                nextShotTime = Time.time + timeBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();

            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, transform.position);

        }
    }

    public void Reload()
    {
        if (!isReloading &&  projectilesRemainingInMag != projectilePerMag)
        {
            StartCoroutine(AnimteReload());
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
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
        projectilesRemainingInMag = projectilePerMag;
    }


    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }
    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }


}
