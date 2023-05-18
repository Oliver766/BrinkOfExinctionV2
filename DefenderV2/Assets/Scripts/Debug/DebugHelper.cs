using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    public bool skipIntro = false;
    public bool instantWeaponCooldown = false;
    public bool disableScenery = false;

    public static DebugHelper debugHelper;
    // Start is called before the first frame update
    void Awake()
    {
        debugHelper = this;

        if (instantWeaponCooldown)
        {
            WeaponsSystems weaponsSystems = FindObjectOfType<WeaponsSystems>();

            weaponsSystems.gatlingCooldown = 0.5f;
            weaponsSystems.laserCooldown = 0.5f;
            weaponsSystems.missileCooldown = 0.5f;
        }
    }
}
