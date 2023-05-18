//Last edited: 24/11/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitcher : MonoBehaviour
{
    [SerializeField] AudioSource aSource;

    /// <summary>
    /// Call at the very start
    /// </summary>
    private void Awake()
    {
        //If there's no AudioSource set, find one attached to the object
        if (aSource == null) aSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Call when the object is enabled/instantiated.
    /// </summary>
    private void OnEnable()
    {
        //If there's an AudioSource, set a random pitch.
        if (aSource != null) aSource.pitch = Random.Range(0.75f, 1.25f);
    }
}
