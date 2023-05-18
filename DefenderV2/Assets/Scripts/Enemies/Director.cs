//Last edited: 02/12/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    //SINGLETON
    public static Director _instance;

    public static Director Instance { get { return _instance; } }

    public int playerScore = 0;

    //Dynamic Difficulty Adjustment
    [Range(0, 1)] public float difficulty = 0.5f;
    [Range(0, 1)] public float savePct = 0.5f;

    //How many enemies are going to spawn in the map?
    public int minEnemies;
    public int maxEnemies;
    //How many are going to be capturers?
    public int maxCapturerAmount;
    //How many humans are going to spawn in the map?
    public int minHumans;
    public int maxHumans;

    //Spawning
    public int enemiesToSpawn;
    public int capsToSpawn;
    public int humansToSpawn;

    //Variable tracking
    public int enemiesLeft;
    public int aliveHumans;
    public int maxListSize = 20;
    public int currentDmg = 0;
    public List<int> playerDamages = new List<int>();
    public List<float> rescuePcts = new List<float>();
    //SDs
    float damageMean = 0;
    float damageStandardDeviation = 15;
    float rescueMean = 0f;
    float rescueStandardDeviation = 0.1f;
    //The upper and lower bound of the Z-Value. Anything below or above its negative or positive respective values is in the bottom/top 25% of values.
    float maxZValue = 0.67f;

    [Header("Win Screen")]
    //WIN SCREEN
    public WinScreenRef winScreen;


    #region MATH
    /// <summary>
    /// Calculate if diffculty should change.
    /// </summary>
    void CalculateEnemyDifficulty()
    {
        //Calculate mean.
        damageMean = 0;
        foreach (int dmg in playerDamages) damageMean += dmg;
        damageMean /= playerDamages.Count;

        //Calculate standard deviation.
        float sigma = 0;
        foreach(int dmg in playerDamages)
        {
            //Get all values of (x - mean)**2 and add them to the total
            float valToAdd = (dmg - damageMean);
            sigma += valToAdd * valToAdd;
        }
        damageStandardDeviation = Mathf.Sqrt(sigma / playerDamages.Count);

        Debug.Log("New damage mean of " + damageMean + ", SD of " + damageStandardDeviation);
        
        //Calculate Z-value to get estimated percentile of player's damage
        float zValue = ((currentDmg - damageMean) / damageStandardDeviation);
        if (zValue > maxZValue || zValue < -maxZValue)
        {
            //Adjust difficulty if in top or bottom 25% percentile of damages.
            Debug.Log("NOW ADJUSTING DIFFICULTY");
            difficulty += currentDmg < damageMean ? 0.2f : -0.2f;
        }
    }

    void CalculateRescueDifficulty(float newRescue)
    {
        //Calculate mean
        rescueMean = 0;
        foreach (float pct in rescuePcts) rescueMean += pct;
        rescueMean /= rescuePcts.Count;

        //Calculate standard deviation
        float sigma = 0;
        foreach(float pct in rescuePcts)
        {
            //Get all values of (x - mean)**2 and add them to the total
            float valToAdd = (pct - rescueMean);
            sigma += valToAdd * valToAdd;
        }
        rescueStandardDeviation = Mathf.Sqrt(sigma / playerDamages.Count);

        Debug.Log("New rescue mean of " + rescueMean * 100 + "%, SD of " + rescueStandardDeviation);

        //Calculate Z-value to get estimated percentile of player's rescue rate.
        float zValue = ((newRescue - rescueMean) / rescueStandardDeviation);
        if (zValue > maxZValue || zValue < -maxZValue)
        {
            //Adjust amount of humans to rescue if in top or bottom 25% percentile of rescue rates.
            //INVERTED - INCREASES WHEN NOT SAVING HUMANS, DECREASES WHEN NOT
            Debug.Log("NOW ADJUSTING HUMAN SPAWN RATE");
            savePct += newRescue < rescueMean ? 0.1f : -0.1f;
        }
    }
    /// <summary>
    /// Update list of damages.
    /// </summary>
    /// <param name="newDamage">The new piece of damage taken over a round.</param>
    void UpdateDamageList(int newDamage)
    {
        //Remove first input damage if max list limit reached.
        if (playerDamages.Count >= maxListSize) playerDamages.RemoveAt(0);
        if (newDamage > 100) newDamage = 100;
        playerDamages.Add(newDamage);
        Debug.Log("CURRENTLY: " + playerDamages.Count + " pieces of damage to reference.");
        CalculateEnemyDifficulty();
    }

    /// <summary>
    /// Update the list of rescue percentages.
    /// </summary>
    /// <param name="newRescue">The new perecentage of humans rescued over a round.</param>
    void UpdateRescueList(float newRescue)
    {
        //Remove first percentage if max list limit reached.
        if (rescuePcts.Count >= maxListSize) rescuePcts.Remove(0);
        rescuePcts.Add(newRescue);
        Debug.Log("CURRENTLY: " + rescuePcts.Count + " rescue percentages to reference.");
        CalculateRescueDifficulty(newRescue);
    }
    #endregion

    /// <summary>
    /// Start a new round of the game. Called when starting a new level.
    /// </summary>
    public void StartLevel()
    {
        //Set amount of NPCs to spawn by lerping.
        enemiesToSpawn = Mathf.RoundToInt(Mathf.Lerp(minEnemies, maxEnemies, difficulty));
        enemiesLeft = enemiesToSpawn;
        capsToSpawn = Mathf.RoundToInt(Mathf.Lerp(0, maxCapturerAmount, difficulty));
        humansToSpawn = Mathf.RoundToInt(Mathf.Lerp(minHumans, maxHumans, savePct));
        aliveHumans = humansToSpawn;
        Debug.Log("NEW ROUND:\n" + enemiesToSpawn + " enemies!\n" + capsToSpawn + " capturers!\n" + humansToSpawn + " humans!");
        //Reset playerDamages to track how much damage taken over a turn.
        currentDmg = 0;
    }

    /// <summary>
    /// Call when the player dies. Store the time they were alive, and recalculate difficulty.
    /// </summary>
    public void PlayerDeath()
    {
        //Update lists of both damage and rescue rate.
        UpdateDamageList(currentDmg);
        UpdateRescueList(0);
    }

    /// <summary>
    /// Call the level to be finished.
    /// </summary>
    public void FinishLevel()
    {
        //Collect variables
        Debug.Log("LEVEL FINISH!");
        //Recalculate difficulty in how many enemies to spawn
        UpdateDamageList(currentDmg);
        //Recalculate amount of humans to spawn.
        if (aliveHumans < 0) aliveHumans = 0;
        float newRescue = aliveHumans / humansToSpawn;
        //Increment difficulty slightly.
        UpdateRescueList(newRescue);
        difficulty += 0.2f;
        //CALL LEVEL FINISH ANIMATIONS
        playerScore += (200 * aliveHumans);
        if (CharacterControl.instance != null) CharacterControl.instance.EndGame();
        Invoke(nameof(WinScreen), 5f);      

    }

    /// <summary>
    /// Call at the very start
    /// </summary>
    void Awake()
    {
        //Set singleton and set it to not destroy
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;
        DontDestroyOnLoad(this);

        //Set default values of tracking variables if starting anew
        if(playerDamages.Count < 3)
        {
            //Set player damages if there's little values to work with
            for(int i = 0; i < 3; i++) playerDamages.Add(50);
            damageMean = 50;
        }
        if(rescuePcts.Count < 3)
        {
            //Set rescue percentages if there's little values to work with
            for(int i = 0; i < 3; i++) rescuePcts.Add(0.5f);
            rescueMean = 0.5f;
        }

    }

    /// <summary>
    /// DEBUG - INPUT KEY PRESS TO GO TO NEXT LEVEL
    /// </summary>
    private void Update()
    {
        //If 'i' pressed, call FinishLevel()
        if (Input.GetKeyDown("i")){
            if (CharacterControl.instance == null) return;
            if (CharacterControl.instance.active) FinishLevel();
        }
    }

    /// <summary>
    /// sets win screen active and starts animation
    /// </summary>
    public void WinScreen()
    {
        Upgrades upgrades = winScreen.upgrades;
        GameObject upgradeText = winScreen.upgradeText;

        // Display 'Upgrade text' if the gamemode is correct and the player is elible to have earnt points
        if(GameController.gameMode == GameModes.StoryMode)
        {
            if (upgrades.AddPoints())
            {
                upgradeText.SetActive(true);
            }
            else
            {
                upgradeText.SetActive(false);
            }
        }
        else
        {
            upgradeText.SetActive(false);
        }

        winScreen.gameObject.SetActive(true);
        Cursor.visible = true;     
    }

}
