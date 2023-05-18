//Last edited: 04/11/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    #region SINGLETON
    private static HumanSpawner _instance;
    public static HumanSpawner Instance { get { return _instance; } }

    /// <summary>
    /// Call before anything else.
    /// </summary>
    private void Awake()
    {
        //Check _instance
        if (_instance != null && _instance != this)
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

    [SerializeField] GameObject humanPrefab;
    [Header("Number of humans to spawn. THIS SHOULD EVENTUALLY BE DYNAMIC.")]
    public int humanCount = 10;

    /// <summary>
    /// Spawn the humans randomly.
    /// </summary>
    public void SpawnHumans()
    {
        //Spawn humanCount number of humans
        RaycastHit hit;
        for (int i = 0; i < humanCount; i++)
        {
            bool inPosition = false;
            Vector3 spawnPos = new Vector3(0,20,0);
            do
            {
                //Random position
                spawnPos = new Vector3(Random.Range(-175, 176), 20, Random.Range(-175, 176));
                //RAYCAST DOWN
                Ray ray = new Ray(spawnPos, -Vector3.up);
                if(Physics.Raycast(ray, out hit))
                {
                    //If there's a raycast collision, check what was hit
                    if (hit.collider.tag == "Terrain" && hit.point.y >= 2.5f)
                    {
                        //If the raycast hit terrain first and was high enough, set the spawn position.
                        inPosition = true;
                        spawnPos = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);
                    }
                }
            } while (!inPosition);
            //Instantiate
            Instantiate(humanPrefab, spawnPos, transform.rotation, transform);
        }
        //Destroy to prevent duplicates being spawned.
        Destroy(this);
    }
}
