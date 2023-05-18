//Last edited: 05/11/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 20;

    /// <summary>
    /// Call on instantiation
    /// </summary>
    void Start()
    {
        //Automatically destroy after 3 seconds if nothing's been hit.
        Destroy(gameObject, 3f);
    }

    /// <summary>
    /// Call every frame.
    /// </summary>
    void Update()
    {
        //Move the bullet forward.
        transform.Translate(-transform.forward * 15 * Time.deltaTime);
    }

    /// <summary>
    /// Call when another object enters this gameObject's trigger
    /// (or in this case, when the bullet hits something)
    /// </summary>
    /// <param name="other">The collider of the object hit.</param>
    private void OnTriggerEnter(Collider other)
    {
        //If the terrain, destroy self. If the player, parse damage AND destroy self.
        if (other.CompareTag("Terrain")) Destroy(gameObject);
        else if (other.CompareTag("Player"))
        {
            //If the player, send damage to the player.
            PlayerHealth.player.TakeDamage(damage);
            Debug.Log("Hit player!");
            Destroy(gameObject);
        }
    }
}
