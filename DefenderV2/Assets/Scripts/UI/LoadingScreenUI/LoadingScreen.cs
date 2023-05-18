using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854
public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    public Animator anim;

    private void Awake()
    {
        if(instance != null)
        {           
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Display or hide the loading screen given a bool
    /// </summary>
    /// <param name="active">Whether to display or hide the screen</param>
    public void DisplayScreen(bool active)
    {
        anim.SetBool("Active", active);
    }
}
