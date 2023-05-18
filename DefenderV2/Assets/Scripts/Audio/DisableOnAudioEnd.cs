//Last edited: 25/11/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnAudioEnd : MonoBehaviour
{
    [SerializeField] AudioSource aSource;

    /// <summary>
    /// Call at the very start
    /// </summary>
    void Awake()
    {
        //If there's no audioSource set, get it.
        if (aSource == null) aSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Call every frame.
    /// </summary>
    void Update()
    {
        //Disable audioSource under certain conditions.
        if (aSource == null) gameObject.SetActive(false);
        else if (!aSource.isPlaying) gameObject.SetActive(false);
    }
}
