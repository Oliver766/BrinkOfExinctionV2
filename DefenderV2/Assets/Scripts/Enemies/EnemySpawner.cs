//Last edited: 20/10/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region SINGLETON
    private static EnemySpawner _instance;
    public static EnemySpawner Instance { get { return _instance; } }

    /// <summary>
    /// Call before anything else.
    /// </summary>
    private void Awake()
    {
        //Check _instance
        if(_instance != null && _instance != this)
        {
            //If there's already an instance, destroy it
            Destroy(this);
        }
        else
        {
            //Set this as an instance
            _instance = this;
        }
    }
    #endregion

    //Required vars to spawn.
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject capturerPrefab;
    [Header("Number of enemies to spawn. SHOULD EVENTUALLY BE DYNAMIC.")]
    public int enemyCount = 10;
    public int capturerCount = 3;
    [Header("Minimum distance from player when spawning. Prevents overcrowding entrance area.")]
    public float startDistFromPlayer = 45;

    public List<Enemy> baseEnemies = new List<Enemy>();
    
    /// <summary>
    /// Spawn the enemies randomly.
    /// </summary>
    public void SpawnEnemies()
    {
        //Start a for loop.
        for(int i = 0; i < enemyCount; i++)
        {
            //Get a random position for the enemy to spawn by assuming it's in a bad position to spawn.
            bool inPosition = false;
            Vector3 spawnPos = new Vector3(0, 0, 0);
            do
            {
                //Perform a do loop to find a random spawn point that isn't near the player.
                spawnPos = new Vector3(Random.Range(25,176),2,Random.Range(25,176));
                //spawnPos += new Vector3(100, 0, 100);
                //If beyond startDist, then set to true.
                if (Vector3.Distance(Vector3.zero, spawnPos) >= startDistFromPlayer) inPosition = true;
            } while (!inPosition);

            //INSTANTIATE based on i.
            GameObject newGO = Instantiate(i < capturerCount ? capturerPrefab : enemyPrefab, spawnPos, transform.rotation, transform);
            baseEnemies.Add(newGO.GetComponent<Enemy>());

        }
        //Destroy to prevent duplicates being spawned.
        Destroy(this);
    }

    /// <summary>
    /// Generate a path for the enemies to move along.
    /// </summary>
    public void SetPath()
    {
        //Go through each of the starter enemies and set a path.
        foreach (Enemy e in baseEnemies) e.SetPath();
    }

    /// <summary>
    /// Set similar targets among the capturer class
    /// </summary>
    public void SetTarget()
    {
        for(int i = 0; i < capturerCount; i++)
        {
            baseEnemies[i].GetComponent<Capturer>().SetTarget();
        }
    }
}
