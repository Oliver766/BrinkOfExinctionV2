using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Playables;
using System.ComponentModel;
using Cinemachine;
/// script by Oliver Lancashire
/// SID 1901981

public class GameController : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// Bools
    /// </summary>
    [Header("Bools")]
    /// <summary>
    /// bool for is playing
    /// </summary>
    public  bool IsPlaying;
    /// <summary>
    /// bool for is restarting
    /// </summary>
    private static bool isRestarting = false;
    /// <summary>
    /// continue story bool
    /// </summary>
    private static bool continueStory = false;
    /// <summary>
    /// allow game start bool 
    /// </summary>
    private bool allowGameStart = false;
    /// <summary>
    /// reference to game modes
    /// </summary>
    [Header("Modes")]
    public static GameModes gameMode;
    /// <summary>
    /// UI screen references
    /// </summary>
    [Header("UI Screens")]
    public GameObject startScreen;
    public GameObject ArcadeloadingScreen;
    public PlanetSelection planetSelectionScreen;
    public CodexController codexController;
    /// <summary>
    /// game object references
    /// </summary>
    [Header("GameObject Parents")]
    public Transform menuObjects;
    public Transform gameObjects;
    /// <summary>
    /// planet reference
    /// </summary>
    [Header("planet reference")]
    public PlanetSetup planetSetup;
    private static int planetSelection;
    /// <summary>
    /// tip dialog references
    /// </summary>
    [Header("Tips & Dialog")]
    public string[] tips;
    public TextMeshProUGUI tipText;
    public Dialog dialog;
    /// <summary>
    /// game controller reference
    /// </summary>
    [Header("Game controller")]
    public static GameController controller = null;
    /// <summary>
    /// animator references
    /// </summary>
    [Header("Animators")]
    public Animator mainmenu;
    public Animator PlanetSelection;
    public Animator lights;
    /// <summary>
    /// time line references
    /// </summary>
    [Header ("Timelines")]
    public PlayableDirector launchSequence;
    public PlayableDirector landingSequence;
    /// <summary>
    /// camera references
    /// </summary>
    [Header("Cameras")]
    public CinemachineVirtualCamera orbitCam;
    public CinemachineVirtualCamera codexCam;
    public CinemachineVirtualCamera planetSelectCam;
    public CinemachineVirtualCamera aircraftCam;
    /// <summary>
    /// level references
    /// </summary>
    private static int level = 0;
    #endregion
    #region Create Instance
    public void Awake()
    {
        // game timescale is 1
        Time.timeScale = 1;
        /// checks if game mode isn't anyother than idle
        if(gameMode != GameModes.StoryMode) gameMode = GameModes.idle;


        if (GameController.controller == null)
        {
            GameController.controller = this;

        }
        else
        {
            if (GameController.controller != this)
            {
                Destroy(GameController.controller.gameObject);

            }
        }
        DontDestroyOnLoad(this.gameObject);

    }
    #endregion

    #region Start
    public void Start()
    {
        /// camera has main focus
        orbitCam.Priority = 1;
        /// audio for main menu
        AudioManager.instance.StopAllSounds();

        // checks if bools are true
        if (DebugHelper.debugHelper.skipIntro || isRestarting)
        {
            // set bool to false
            DebugHelper.debugHelper.skipIntro = false;
            // bool set to false
            isRestarting = false;
            // run function
            StartFromPlanet();
        }
        else
        {
            // object is set to false
            LoadingScreen.instance.DisplayScreen(false);

            // play sounds
            AudioManager.instance.Play("Menu Intro");
            AudioManager.instance.PlayWithDelay("Menu Loop", AudioManager.instance.GetSoundLength("Menu Intro"));
        }
        // checks if bool is true
        if (continueStory)
        {

            Debug.LogWarning("Continue story true");
            // bool set to false
            continueStory = false;
            // add one level
            level++;
            // run function
            BeginStorymode();
        }
        else
        {
            Debug.LogWarning("Continue story false");
        }
    }
    #endregion
    #region Update
    private void Update()
    {
        // checks if there has been input and is game mode is arcade
        if (Input.GetKeyDown(KeyCode.Space) && gameMode == GameModes.ArcadeMode && allowGameStart)
        {
            // game object is set to false
            ArcadeloadingScreen.SetActive(false);
            // set bool to true      
            allowGameStart = false;
            // select planet at randon
            planetSelection = Random.Range(0, planetSetup.planets.Length);
            // run function
            StartGame();
        }
   
    }
    #endregion
    /// <summary>
    /// start game function
    /// </summary>
    #region Start Game
    public void StartGame()
    {
        // set to false
        menuObjects.gameObject.SetActive(false);
        // set active
        gameObjects.gameObject.SetActive(true);
        // planet selection function
        planetSetup.SelectPlanet(planetSelection);
        // play sequence
        landingSequence.Play();
        //checks if game mode is story mode
        if(gameMode == GameModes.StoryMode)
        {
            // start coroutine
            StartCoroutine(dialog.TriggerDialog(planetSelection, false, 2f));
        }
        // invoke function method
        Invoke(nameof(EnableControls), 17f);
    }
    #endregion
    #region Endable Controls
    /// <summary>
    /// enable controls function
    /// </summary>
    private void EnableControls()
    {
        // set screen to false
        LoadingScreen.instance.DisplayScreen(false);
        //enable controls
        CharacterControl.instance.EnableControls(true);
    }
    #endregion
    #region Begin Story Modes
    /// <summary>
    /// begin story mode function
    /// </summary>
    public void BeginStorymode()
    {
        // play animation
        mainmenu.Play("HideMenu");

        //Switch from orbit cam to planet select cam
        orbitCam.Priority = 0;
        planetSelectCam.Priority = 1;
        // run method
        planetSelectionScreen.DisplayPlanets(level);
    }
    #endregion
    #region BeinArcadeMode
    /// <summary>
    ///  being arcade mode function
    /// </summary>
    public void BeginArcadeMode()
    {
        // checks if bool is true
        if(!IsPlaying)
        {
            // set to true
            IsPlaying = true;
            // gsme mode is arcade mode
            gameMode = GameModes.ArcadeMode;
            // play aniamtiom
            mainmenu.Play("HideMenu");
            // play animation
            lights.SetBool("Lights", true);

            //Switch from orbit cam to aircraft cam
            orbitCam.Priority = 0;
            aircraftCam.Priority = 1;
            // play sequence
            launchSequence.Play();
            // run function
            HideCursor();
            // run function with delay
            Invoke(nameof(ShowArcadeLoadingScreen), 13f);
        }
    }
    #endregion
    #region Codex
    /// <summary>
    /// codex function
    /// </summary>
    public void Codex()
    {
        // play animation
        mainmenu.Play("HideMenu");
        // open dispkay
        codexController.OpenDisplay();
        //Switch from orbit cam to codex cam
        orbitCam.Priority = 0;
        codexCam.Priority = 1;
    }
    #endregion
    #region ShowMenu
    /// <summary>
    /// show menu function
    /// </summary>
    public void ShowMenu()
    {
        // play animatiom
        mainmenu.Play("ShowMenu");

        //Switch from planet select / codex cam to planet select orbit cam
        orbitCam.Priority = 1;
        codexCam.Priority = 0;
        planetSelectCam.Priority = 0;
    }
    #endregion
    #region Show Arcade Loading Screen
    /// <summary>
    /// show loading screen function
    /// </summary>
    private void ShowArcadeLoadingScreen()
    {
        // set active
        ArcadeloadingScreen.SetActive(true);
        // randomise displaying tips
        tipText.text = tips[Random.Range(0, tips.Length)];
        // set to true
        allowGameStart = true;
    }
    #endregion
    #region PlanetLoading
    /// <summary>
    /// planet loading function
    /// </summary>
    /// <param name="planet"></param>
    public void planetLoading(int planet)
    {
        // if bool isn't true
        if (!IsPlaying)
        {
            // set to true
            IsPlaying = true;
            // game mode is story mode
            gameMode = GameModes.StoryMode;
            // play animation
            lights.SetBool("Lights", true);
            //Switch from planet select cam to aircraft cam
            planetSelectCam.Priority = 0;
            aircraftCam.Priority = 1;
            //play sequence
            launchSequence.Play();
            // run function
            HideCursor();
            // start coroutine
            StartCoroutine(dialog.TriggerDialog(planet, true, 13f));
            //planet selection equals planet
            planetSelection = planet;
        }
    }
    #endregion
    #region HideCursor
    /// <summary>
    /// hide cursor function
    /// </summary>
    private void HideCursor()
    {
        // cursor lockstate is confined
        Cursor.lockState = CursorLockMode.Confined;
        // cursor is not visible
        Cursor.visible = false;
    }
    #endregion
    #region LoadMainMenu
    /// <summary>
    /// load mainmenu function
    /// </summary>
    public void LoadMainMenu()
    {
        // audio functons
        AudioManager.instance.StopAllSounds();
        AudioManager.instance.CancelAllPlayWithDelay();
        // load scenes
        SceneManager.LoadScene(0);
    }
    #endregion
    #region RestartGame
    /// <summary>
    /// restart game function
    /// </summary>
    public void RestartGame()
    {
        // loading screen is set active
        LoadingScreen.instance.DisplayScreen(true);
        // bool is true
        isRestarting = true;
        // function ran with delay
        Invoke(nameof(LoadMainMenu), 2f);
    }
    #endregion
    #region Continue
    /// <summary>
    /// continue function
    /// </summary>
    public void Continue()
    {
        // loading screen is set active
        LoadingScreen.instance.DisplayScreen(true);
        // checks is game mode is story mode and bool is true
        if (gameMode == GameModes.StoryMode) continueStory = true;
        // run function with delay
        Invoke(nameof(LoadMainMenu), 2f);
    }
    #endregion

    #region Quit
    /// <summary>
    /// quit function
    /// </summary>
    public void Quit()
    {
        // quit fucntion
        Application.Quit();
    }
    #endregion
    #region start from planet
    /// <summary>
    /// start from planet function
    /// </summary>
    public void StartFromPlanet()
    {
        // hide cursor function
        HideCursor();
        // cursor lokc state is confined
        Cursor.lockState = CursorLockMode.Confined;
        // objects set to false
        menuObjects.gameObject.SetActive(false);
        // objects set active
        gameObjects.gameObject.SetActive(true);
        // run function
        planetSetup.SelectPlanet(planetSelection);
        // change launch sequence start time
        launchSequence.time = launchSequence.duration;
        launchSequence.Evaluate();
        // chnange landing sequence start time
        landingSequence.time = landingSequence.duration;
        landingSequence.Evaluate();
        // destroy object
        Destroy(landingSequence.GetComponent<Animator>());
        // run function with delay
        Invoke(nameof(EnableControls), 5f);
    }
    #endregion
}
#region Enum
public enum GameModes
{
    ArcadeMode,
    StoryMode,
    idle,
}
#endregion