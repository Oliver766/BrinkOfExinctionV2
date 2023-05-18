//Last edited: 02/12/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public GameObject explosionPrefab;

    public bool alive = true;
    public int idNum;
    public int score;

    /// <summary>
    /// Kill the entity - different from making them invisible or destroying them through code.
    /// </summary>
    public void Kill()
    {
        //Set alive to false, make an explosion, and destroy this entity.
        alive = false;
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation, transform.parent);
        Destroy(explosion, 1f);
        Destroy(this);
    }
}
