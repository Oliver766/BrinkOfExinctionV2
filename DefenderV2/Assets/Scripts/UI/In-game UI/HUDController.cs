using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// script by Oliver Lancashire
// SID 190981  
public class HUDController : MonoBehaviour
{
    #region fields
    /// <summary>
    /// Bools for pause menu
    /// </summary>
    [Header("Bools")]
    public bool GameIsPaused = false;
    public bool canPause = false;
    /// <summary>
    /// animation for pause menu
    /// </summary>
    [Header("Gameobjects")]
    public Animator pauseAnim;
    public Animator HUDAnim;
    [Header("Game Controller")]
    /// <summary>
    /// game controller reference
    /// </summary>
    public GameController controller;
    #endregion

    #region UpDate 
    void Update()
    {
        // there is input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // checks if bool is true
            if (GameIsPaused)
            {
                // run function
                Resume();
            }
            // checks if bool is true ahd false
            else if (!GameIsPaused && canPause)
            {
                // run function
                Pause();
            }
        }
    }
    #endregion
    #region Resume Function
    /// <summary>
    /// resume function
    /// </summary>
    public void Resume()
    {
        // pause sound
        AudioManager.instance.PauseAllSounds(true);
        // enable controls
        CharacterControl.instance.EnableControls(true);
        //play animation
        pauseAnim.SetBool("Paused", false);
        // curser is set to fause
        Cursor.visible = false;
        // cursor is confined
        Cursor.lockState = CursorLockMode.Confined;
        // game runs
        Time.timeScale = 1f;
        // game is unpaused
        GameIsPaused = false;

    }
    #endregion


    #region Pause Function
    /// <summary>
    /// pause function
    /// </summary>
    public void Pause()
    {
        // pause sound is false
        AudioManager.instance.PauseAllSounds(false);
        // disable controls
        CharacterControl.instance.EnableControls(false);
        // run animation
        pauseAnim.SetBool("Paused", true);
        // curser is visible
        Cursor.visible = true;
        // game is paused
        Time.timeScale = 0f;
        // game is paused
        GameIsPaused = true;
    }
    #endregion
}
