using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;

public class WeaponsSystems : MonoBehaviour
{
    [Header("Gatling gun attributes")]

    public int gatlingDamage = 2;
    public float gatlingFireRate = 15f;
    public float gatlingCooldown = 5f;

    [Header("Laser attributes")]

    public int laserDamage = 5;
    public float laserRange = 15f;
    public float laserFireRate = 30f;
    public float laserCooldown = 10f;

    [Header("Missile attributes")]

    public int numberOfMissiles = 2;
    public float lockOnRange = 50f;
    public float missileCooldown = 30f;

    [Header("Weapon Firepoints")]

    public Transform gatlingFirePoint;
    public Transform laserFirePoint;
    public Transform missileFirePoint;

    [Header("Effects & Feedback")]

    public Animator hitMarkerAnim;
    public Animator weaponsAnim;
    public Animator HUDAnim;
    public ParticleSystem muzzleFlash;
    public GameObject laserBeam;
    public GameObject laserImpact;

    [System.NonSerialized]
    public AudioManager audioManager;

    private List<Transform> lockedOnTargets = new List<Transform>();

    [System.NonSerialized]
    public WeaponType weaponType;

    [Header("Prefabs")]

    public GameObject missilePrefab;
    public GameObject bulletPrefab;

    [Header("UI")]

    public Image[] weaponsUI;
    public Image[] cooldownUI;
    public GameObject[] weaponsCursors;

    private bool active;

    private bool gatlingAvailable = true;
    private bool laserAvailable = true;
    private bool missilesAvailable = true;

    private bool gatlingIsFiring = false;
    private bool laserIsFiring = false;

    private float nextTimeToFireGatling = 0f;
    private float nextTimeToFireLaser = 0f;

    private float laserPower = 0.1f;

    private CharacterControl characterControl;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        characterControl = GetComponent<CharacterControl>();
    }

    public void EnableWeapons(bool enabled)
    {
        active = enabled;

        if (!enabled)
        {
            StopFiring();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            // If player is firing, don't let the player switch weapons

            if (!Input.GetKey(KeyCode.Space) && !Input.GetMouseButton(0))
            {
                // Switch weapons with the number keys

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    // Change the weapon type
                    weaponType = WeaponType.Gatling;

                    weaponsAnim.SetBool("Laser", false);
                    weaponsAnim.SetBool("Minigun", true);
                    HUDAnim.SetInteger("WeaponSelection", 0);

                    // Display the correct UI
                    weaponsCursors[0].SetActive(true);
                    weaponsCursors[1].SetActive(false);
                    weaponsCursors[2].SetActive(false);

                    // Stop any rogue sounds playing
                    audioManager.Stop("LockedOn");
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    // Change the weapon type
                    weaponType = WeaponType.Laser;

                    weaponsAnim.SetBool("Minigun", false);
                    weaponsAnim.SetBool("Laser", true);
                    HUDAnim.SetInteger("WeaponSelection", 1);

                    // Display the correct UI
                    weaponsCursors[0].SetActive(false);
                    weaponsCursors[1].SetActive(true);
                    weaponsCursors[2].SetActive(false);

                    // Stop any rogue sounds playing
                    audioManager.Stop("LockedOn");
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    // Change the weapon type
                    weaponType = WeaponType.Missiles;

                    weaponsAnim.SetBool("Laser", false);
                    weaponsAnim.SetBool("Minigun", false);
                    HUDAnim.SetInteger("WeaponSelection", 2);

                    // Display the correct UI
                    weaponsCursors[0].SetActive(false);
                    weaponsCursors[1].SetActive(false);
                    weaponsCursors[2].SetActive(true);
                }
            }

            // Handle space key down

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                // Perform a different action depending on weapon type selected

                switch (weaponType)
                {
                    case WeaponType.Gatling:

                        // Ensure the weapon is not on cooldown
                        if (gatlingAvailable)
                        {
                            // Initiate gatling chargeup
                            Invoke(nameof(FireGatling), 1.6f);
                            audioManager.Play("GatlingStart");
                            HUDAnim.SetBool("GatlingActive", true);
                        }
                        else
                        {
                            // Play an error sound for weapons on cooldown
                            audioManager.Play("CoolDown");
                        }

                        break;

                    case WeaponType.Laser:

                        // Ensure the weapon is not on cooldown
                        if (laserAvailable)
                        {
                            // Set the laser as active
                            audioManager.Play("LaserLoop");
                            laserBeam.SetActive(true);
                            laserPower = 0.1f;
                            laserIsFiring = true;
                            HUDAnim.SetBool("LaserActive", true);
                        }
                        else
                        {
                            // Play an error sound for weapons on cooldown
                            audioManager.Play("CoolDown");
                        }

                        break;

                    case WeaponType.Missiles:

                        // Ensure the weapon is not on cooldown and there is at least one target locked on
                        if (missilesAvailable && lockedOnTargets.Count > 0)
                        {
                            // Fire the missiles and reset the cooldown
                            missilesAvailable = false;
                            cooldownUI[2].fillAmount = 0;
                            StartCoroutine(FireMissiles());
                        }
                        else
                        {
                            // Play an error sound for weapons on cooldown
                            audioManager.Play("CoolDown");
                        }

                        break;
                }
            }

            // Handle space key held

            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                // Perform a different action depending on weapon type selected

                switch (weaponType)
                {
                    case WeaponType.Gatling:

                        // Ensure the weapon isn't firing faster than it's hitrate
                        if (Time.time >= nextTimeToFireGatling && gatlingIsFiring)
                        {
                            // Set how long it should be before we fire again
                            nextTimeToFireGatling = Time.time + 1f / gatlingFireRate;
                            FireGatling();
                        }

                        break;

                    case WeaponType.Laser:

                        // Ensure the weapon isn't firing faster than it's hitrate
                        if (Time.time >= nextTimeToFireLaser && laserAvailable)
                        {
                            // Set how long it should be before we fire again
                            nextTimeToFireLaser = Time.time + 1f / laserFireRate;
                            FireLaser();
                        }

                        // Ensure the weapon is firing
                        if (laserIsFiring)
                        {
                            // Ramp up the laser firepower over time, to it's maximum amount
                            laserPower += Time.deltaTime /2;
                            laserPower = Mathf.Clamp(laserPower, 0.1f, 2);

                            SetLaserVisual();
                        }

                        break;
                }
            }

            // Handle space key up

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
            {
                StopFiring();
            }

            // Cooldown rechargers

            if (!gatlingAvailable)
            {
                gatlingAvailable = (RechargeCooldown((int)WeaponType.Gatling, gatlingCooldown));
            }

            if (!laserAvailable)
            {
                laserAvailable = (RechargeCooldown((int)WeaponType.Laser, laserCooldown));
            }

            if (!missilesAvailable)
            {
                missilesAvailable = (RechargeCooldown((int)WeaponType.Missiles, missileCooldown));
            }
        }
    }

    public void StopFiring()
    {
        // Perform a different action depending on weapon type selected

        switch (weaponType)
        {
            case WeaponType.Gatling:

                // Ensure the weapon is available
                if (gatlingAvailable)
                {
                    // Play the correct sounds
                    audioManager.Stop("GatlingStart");
                    audioManager.Stop("GatlingFire");
                    audioManager.Play("GatlingStop");
                    HUDAnim.SetBool("GatlingActive", false);
                }

                // Ensure the weapon is firing (prevents cooldown triggering before weapon is actually active and firing)
                if (gatlingIsFiring)
                {
                    // Reset the weapon cooldown
                    gatlingIsFiring = false;
                    gatlingAvailable = false;
                    cooldownUI[0].fillAmount = 0;
                }

                break;

            case WeaponType.Laser:

                // Ensure the weapon is firing
                if (laserAvailable && laserIsFiring)
                {
                    audioManager.Stop("LaserLoop");

                    // Reset the weapon cooldown
                    laserIsFiring = false;
                    laserAvailable = false;
                    cooldownUI[1].fillAmount = 0;

                    // Disable the visuals for the laser
                    laserBeam.SetActive(false);
                    laserImpact.SetActive(false);

                    HUDAnim.SetBool("LaserActive", false);
                }

                break;
        }
    }

    /// <summary>
    /// Recharge the cooldown on a weapon, returning true once complete
    /// </summary>
    /// <param name="weaponID">The weapon to recharge</param>
    /// <param name="weaponCoolDown">The cooldown length of the weapon</param>
    /// <returns></returns>
    private bool RechargeCooldown(int weaponID, float weaponCoolDown)
    {
        // Refill the cooldown UI over time
        cooldownUI[weaponID].fillAmount += 1 / weaponCoolDown * Time.deltaTime;

        // When the cooldown has passed, set the weapon as available
        if (cooldownUI[weaponID].fillAmount >= 1)
        {
            cooldownUI[weaponID].fillAmount = 1;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  Set the laser's length and hit animation visibility
    /// </summary>
    private void SetLaserVisual()
    {
        // Check if the laser hit anything
        if (Physics.Raycast(laserFirePoint.position, laserFirePoint.transform.forward, out RaycastHit hit, laserRange))
        {
            // If the laser hit, set it's length to the distance between the firepoint and hit point
            laserBeam.transform.localScale = new Vector3(laserPower, Vector3.Distance(laserFirePoint.position, hit.point) / 30, laserPower);
            laserImpact.transform.localScale = new Vector3(laserPower / 2, 1, laserPower / 2);

            // Enable the laser impact visibility
            laserImpact.SetActive(true);
            // Set the laser impact position
            laserImpact.transform.position = hit.point - laserImpact.transform.up * 0.5f;
        }
        else
        {
            // If the laser hasn't hit anything, set it to it's default length and disable the laser impact visibility
            laserImpact.SetActive(false);
            laserBeam.transform.localScale = new Vector3(laserPower, laserRange/15f, laserPower);
        }
    }

    /// <summary>
    /// Fire the gatling
    /// </summary>
    private void FireGatling()
    {
        // Ensure space is held before proceeding
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {

            // If gatling isn't currently firing, set the bool and play the relevant sound
            if (!gatlingIsFiring)
            {
                gatlingIsFiring = true;

                audioManager.Stop("GatlingStart");
                audioManager.Play("GatlingFire");
            }

            // Play the relevant particle effect
            muzzleFlash.Play();

            // Spawn the bullet prefab
            GameObject bulletGameoject = Instantiate(bulletPrefab, gatlingFirePoint.position, gatlingFirePoint.rotation, null);

            Bullet bullet = bulletGameoject.GetComponentInChildren<Bullet>();

            bullet.SetWeaponsVariable(this);
        }
    }

    /// <summary>
    /// Fire the laser
    /// </summary>
    private void FireLaser()
    {

        // Ensure space is held before proceeding
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            // Check if the laser is hitting with a raycast
            if (Physics.Raycast(laserFirePoint.position, laserFirePoint.transform.forward, out RaycastHit hit, laserRange))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    // If it is an enemy, play the hit sound and hitmarker on the UI
                    hitMarkerAnim.Play("Hit");
                    audioManager.Play("LaserImpact");

                    // Do damage to the enemy based on the current power of the laser
                    hit.transform.SendMessage("TakeDamage", (int)(laserDamage * (laserPower / 2)));
                }
            }
        }
    }

    /// <summary>
    /// Fire missiles
    /// </summary>
    private IEnumerator FireMissiles()
    {
        // Reset the missile counter
        //int missileCounter = 0;

        // Make a copy of the locked on targets (avoids errors if amount of targets changes as the missiles are being fired)
        List<Transform> lockedOnTargetsCopy = new List<Transform>(lockedOnTargets);

        // Loop through locked on enemies
        foreach (Transform enemy in lockedOnTargetsCopy)
        {
            // Ensure we haven't reached the maximum number of missiles
            //if (missileCounter < numberOfMissiles)
            //{
                // Instantiate a missile
                GameObject missile = Instantiate(missilePrefab, missileFirePoint.position, missileFirePoint.rotation);

                // Set the missile target as the enemy
                missile.GetComponent<Missile>().target = enemy.Find("Model");

                // Remove the target from the array of locked on targets
                RemoveTargetForLockOn(enemy);

                yield return new WaitForSeconds(0.5f);

                // Increment the missile counter
                //missileCounter++;
            //}
        }
    }

    /// <summary>
    /// Add the target to the lockedOnTargets array for use with missiles ability
    /// </summary>
    /// <param name="target">The target to add to the array</param>
    public void AddTargetForLockOn(Transform target)
    {
        // Check to make sure the target doesn't already exist in the array
        if(!lockedOnTargets.Find(x => x == target) && lockedOnTargets.Count < numberOfMissiles)
        {
            lockedOnTargets.Add(target);
        }

        HUDAnim.SetInteger("LockOns", lockedOnTargets.Count);
    }

    /// <summary>
    /// Remove the target from the lockedOnTargets array
    /// </summary>
    /// <param name="target"></param>
    public void RemoveTargetForLockOn(Transform target)
    {
        if(lockedOnTargets.Count > 0)
        {
            lockedOnTargets.Remove(target);
        }

        if (lockedOnTargets.Count == 0)
        {
            audioManager.Stop("LockedOn");
        }
        HUDAnim.SetInteger("LockOns", lockedOnTargets.Count);
    }

    /// <summary>
    /// Adjust variables according to their string names (For use with modular upgrade system)
    /// </summary>
    /// <param name="variableName">The name of the variable to be changed</param>
    /// <param name="amount">The value the variable should be changed to</param>
    public void AdjustProperty(string variableName, float amount)
    {
        FieldInfo field = this.GetType().GetField(variableName);

        if(field != null)
        {
            if (field.FieldType == typeof(int))
            {
                field.SetValue(this, (int)amount);
            }
            else if (field.FieldType == typeof(float))
            {
                field.SetValue(this, amount);
            }
        }
        else
        {
            Debug.LogWarning("Variable " + variableName + " not found.");
        }
    }
}

/// <summary>
/// WeaponType enum, publicly accessible in other scripts
/// </summary>
public enum WeaponType
{
    Gatling,
    Laser,
    Missiles
}
