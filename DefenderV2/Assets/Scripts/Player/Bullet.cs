using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854
public class Bullet : MonoBehaviour
{
    private WeaponsSystems weaponsSystems;

    // Start is called before the first frame update
    void Start()
    {
        // Give the bullet a slightly random rotation on spawn
        transform.parent.rotation *= Quaternion.Euler(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));

        // Destroy the bullet after 3 seconds (presuming it hasn't hit anything before then)
        Destroy(transform.parent.gameObject, 3f);
    }

    /// <summary>
    /// Set weaponsSystems variable when the bullet spawns (Set from weaponsSystems)
    /// </summary>
    /// <param name="weaponsSystems"></param>
    public void SetWeaponsVariable(WeaponsSystems weaponsSystems)
    {
        this.weaponsSystems = weaponsSystems;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the bullet forward
        transform.parent.Translate(Vector3.forward * 20 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            // If it is an enemy, play the hit sound and hitmarker on the UI
            weaponsSystems.hitMarkerAnim.Play("Hit");
            weaponsSystems.audioManager.Play("GatlingImpact");

            // Do damage to the enemy
            other.SendMessage("TakeDamage", weaponsSystems.gatlingDamage);
        }

        // Destroy the bullet when it hits anything other than the player
        if (!other.CompareTag("Player"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
