//DEPRECATED - Renamed 14/10/21 to prevent confusion between scripts. SAVED FOR REFERENCE, DO NOT USE!

//Last Edited: 04/10/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    //Vars
    public int health;
    public int defence;
    public bool alive = true;
    public int enemyID;

    //Navigation
    NavMeshAgent nma;
    Vector3 startPos;
    [SerializeField]
    Vector3 destPos;

    /// <summary>
    /// Kill the enemy - different making them invisible or destroying them through code.
    /// </summary>
    public void Kill()
    {
        //Set alive to false and destroy this enemy.
        alive = false;
        Destroy(this);
    }

    /// <summary>
    /// Call at start of program
    /// </summary>
    void Start()
    {
        //Get child index for future referencing.
        enemyID = transform.GetSiblingIndex();
        //Set up navigation
        startPos = new Vector3(transform.localPosition.x, -0.09f, transform.localPosition.z);
        transform.localPosition = startPos;
        destPos = new Vector3(transform.localPosition.x + Random.Range(-30, 31), -0.09f, transform.localPosition.z + Random.Range(-30, 31));
        nma = GetComponent<NavMeshAgent>();
        nma.SetDestination(destPos);
    }

    /// <summary>
    /// Call when this script is destroyed
    /// </summary>
    private void OnDestroy()
    {
        //Check alive bool
        if (!alive)
        {
            //If killed, get tile this is on and all the other tiles.
            Transform myTile = transform.parent.parent;
            Transform terrain = myTile.parent;
            Transform[] tiles = terrain.GetComponentsInChildren<Transform>();
            foreach (Transform t in tiles)
            {
                //Go through each tile - if it's the tile this enemy is on, continue
                if (t == myTile) continue;
                else
                {
                    //If not this tile, scan for enemies
                    EnemyNavMesh[] enemies = t.GetComponentsInChildren<EnemyNavMesh>();
                    foreach (EnemyNavMesh e in enemies)
                    {
                        //Go through each enemy - if the same ID, destroy it and break.
                        if (e.enemyID == enemyID) Destroy(e);
                        break;
                    }
                }
            }
        }
        //Destroy attached gameObject
        Destroy(this.gameObject);
    }
}
