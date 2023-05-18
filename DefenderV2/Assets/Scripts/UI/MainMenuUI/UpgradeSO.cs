using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854

/// <summary>
/// Class for the upgrade scriptable objects
/// </summary>
[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrade", order = 1)]
public class UpgradeSO : ScriptableObject
{
    public string variableToAdjust;
    public float amount;

    public string description;

    // Adjust variable in weaponSystems given the variable name and amount
    public void ApplyUpgrade(WeaponsSystems weaponsSystems)
    {
        weaponsSystems.AdjustProperty(variableToAdjust, amount);
    }
}
