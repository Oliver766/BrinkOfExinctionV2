using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854
public class Recticle : MonoBehaviour
{
    public GameObject target;
    private float lockOnRange;

    private WeaponsSystems weapons;
    private AudioManager audioManager;

    private bool isLockedOn = false;

    /// <summary>
    /// Check to see if the enemy is visible on screen
    /// </summary>
    /// <returns></returns>
    private bool IsVisible()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        var point = transform.position;

        foreach (var plane in planes)
        {
            if(plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }
        return true;
    }

    private void Awake()
    {
        weapons = FindObjectOfType<WeaponsSystems>();
        lockOnRange = weapons.lockOnRange;

        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (weapons.weaponType == WeaponType.Missiles && Vector3.Distance(Camera.main.transform.position, transform.position) < lockOnRange && IsVisible())
        {
            // Enable the target recticle
            target.SetActive(true);

            if (!isLockedOn)
            {
                isLockedOn = true;
                Invoke(nameof(LockOn), 1f);
                audioManager.Play("LockOnStart");
            }

            // Ensure the target is facing the player
            target.transform.LookAt(Camera.main.transform.position, Vector3.up);
            target.transform.rotation = target.transform.rotation * Quaternion.Euler(90, 0, 0);
        }
        else
        {
            // Disable the target recticle
            target.SetActive(false);

            if (isLockedOn)
            {
                isLockedOn = false;
                weapons.RemoveTargetForLockOn(transform);
            }
        }
    }

    private void LockOn()
    {
        if (IsVisible() && weapons.weaponType == WeaponType.Missiles)
        {
            // Add the target to a list of locked on enemies
            weapons.AddTargetForLockOn(transform);
            audioManager.Play("LockedOn");
        }
    }
}
