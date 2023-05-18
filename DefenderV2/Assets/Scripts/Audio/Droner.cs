//Last edited: 25/11/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droner : MonoBehaviour
{
    [SerializeField] AudioSource droner;

    /// <summary>
    /// Call every frame.
    /// </summary>
    void Update()
    {
        if(CharacterControl.instance != null)
        {
            //If there's a character controller to reference, set the engine noise to be active if the player can move.
            droner.enabled = CharacterControl.instance.active;
        }
    }
}
