using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854
public class PlayerCollision : MonoBehaviour
{
    public bool canCapture = false;

    /// <summary>
    /// Destroy the player if they collide with another object
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.CompareTag("Terrain") || collision.transform.CompareTag("Water"))
        {
            Debug.Log("Hit terrain");
            PlayerHealth.player.TakeDamage(100);
        }

        Enemy enemy = collision.transform.GetComponent<Enemy>();

        // If the collision was with an enemy, destroy them and take a large amount of damage
        if (enemy)
        {
            Debug.Log("Hit enemy");

            enemy.Kill();
            PlayerHealth.player.TakeDamage(80);
        }
    }

    /// <summary>
    /// Attempt to capture a human
    /// </summary>
    /// <returns></returns>
    public bool CaptureHuman()
    {
        if(canCapture)
        {
            Debug.Log("Rescued human!");

            //Play captured sound
            AudioManager.instance.Play("Capture");
            return true;
        }
        else
        {
            return false;
        }
    }
}
