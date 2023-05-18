using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script by Matthew Harris
// SID 1808854

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public GameObject destroyedModel;
    public GameObject playerModel;
    public Animator deathCameraAnim;

    public Image healthBar;
    public GameObject gameoverscreen;

    public GameController controller;

    public static PlayerHealth player;

    private int fullHealth;
    private bool isDead = false;

    /// <summary>
    /// Take a set amount of damage
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        // Parse damage to Director

        if(Director.Instance != null)
        {
            Director.Instance.currentDmg += amount;
        }

        // Ensure the damage can be subtracted from the health, and if not, kill the player
        if (health - amount <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                Die();
            }
        }
        else
        {
            health -= amount;
            
            AudioManager.instance.Play("Player Hit");
            healthBar.fillAmount = (float)health / (float)fullHealth;
        }
    }

    /// <summary>
    /// Kill the player
    /// </summary>
    private void Die()
    {
        // Stop all sound effects and cancel any sound in the queue
        AudioManager.instance.StopAllSoundEffects();
        AudioManager.instance.CancelAllPlayWithDelay();

        // Disable player control
        CharacterControl.instance.EnableControls(false);

        playerModel.SetActive(false);
        destroyedModel.SetActive(true);

        //Tell director player is dead.

        Director.Instance.PlayerDeath();

        deathCameraAnim.Play("ZoomOut");
        Invoke(nameof(deathScreen), 3f);       

    }

    private void Start()
    {
        player = this;
        fullHealth = health;
    }

    /// <summary>
    /// function for death screen
    /// </summary>
    private void deathScreen()
    {
        gameoverscreen.SetActive(true);
        Cursor.visible = true;    
    }  
}
