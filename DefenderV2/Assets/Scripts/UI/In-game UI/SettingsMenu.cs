using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

// script by Oliver Lancashire
// SID 190981        

public class SettingsMenu : MonoBehaviour
{
    #region  Fields
    /// <summary>
    /// music references
    /// </summary>
    [Header("Volume")]
    public AudioMixer musicMixer;
    public AudioMixer soundMixer;
    /// <summary>
    /// setting menus reference
    /// </summary>
    [Header("instance")]
    public static SettingsMenu settingsMenu;
    /// <summary>
    /// text and image arrays
    /// </summary>
    [Header("Options dropdown")]
    public Image[] images;
    public Image[] imagesWhite;
    public Image[] imagesGreen;
    public TextMeshProUGUI[] Texts;
    public TextMeshProUGUI[] Textwhite;
    public TextMeshProUGUI[] TextsGreen;

    public Image scoreBox;
    public Image miniMapBoarder;
    /// <summary>
    /// colour reference
    /// </summary>
    [Header("Colour")]
    public Color color;




    #endregion

    #region Setting Intance
    /// <summary>
    /// setting the settings don't destry at load in away
    /// </summary>
    public void Awake()
    {
        // setting instance
        settingsMenu = this;

        // don't destroy on load
        if (SettingsMenu.settingsMenu == null)
        {
            SettingsMenu.settingsMenu = this;

        }
        else
        {
            if (SettingsMenu.settingsMenu != this)
            {
                Destroy(SettingsMenu.settingsMenu.gameObject);

            }
        }
        //DontDestroyOnLoad(this.gameObject);


        
    }
    #endregion

    public void Update()
    {
        
    }

    #region set music volume
    /// <summary>
    /// set volume float
    /// </summary>
    /// <param name="Volume"></param>
    public void setMusicVolume(float Volume)
    {
        // set volume
        musicMixer.SetFloat("Volume", Volume);
    }
    #endregion

    #region set sound volume
    public void setSoundVolume(float Volume)
    {
        // set sound volume 
        soundMixer.SetFloat("Volume", Volume);
    }
    #endregion
    #region UI Changer
    /// <summary>
    /// updates all images in game on selection. matches up value to dropdown list value, count's through lists and applys to all images and text
    /// </summary>
    /// <param name="val"></param>
    public void UIChangerImage(int val)
    {
        // checks value select from 0-5
        if(val == 0)
        {
            // sets colour value
            Color c = scoreBox.color;
            // alpha of image
            c.a = 0.5f;
            // set image
            scoreBox.color = c;
            // sets colour value
            Color d = miniMapBoarder.color;
            // alpha of image
            d.a = 0.5f;
            // set image
            miniMapBoarder.color = d;
            // sets colour of all of the images
            for (int i = 0; i < imagesGreen.Length; i++)
            {
                imagesGreen[i].color = Color.green;
            }
            for (int i = 0; i < imagesWhite.Length; i++)
            {
                imagesWhite[i].color = Color.white;
            }

 
            Debug.Log("Default");
        }
        if (val == 1)
        {
          
            Debug.Log("Alpha");

            // counts throuhg list array
            for (int i = 0; i < images.Length; i++)
            {
                // sets colour for images
                images[i].color = Color.red;
                // sets colour value
                Color c = scoreBox.color;
                // alpha of image
                c.a = 0.5f;
                // set image
                scoreBox.color = c;
                // sets colour value
                Color d = miniMapBoarder.color;
                // alpha of image
                d.a = 0.5f;
                // set image
                miniMapBoarder.color = d;
                Debug.Log("change");


            }

        }
        if (val == 2)
        {
            // counts throuhg list array
            for (int i = 0; i < images.Length; i++)
            {
                // sets colour for images
                images[i].color = Color.green;
                // sets colour value
                Color c = scoreBox.color;
                c.a = 0.5f;
                // set image
                scoreBox.color = c;
                // sets colour value
                Color d = miniMapBoarder.color;
                // alpha of image
                d.a = 0.5f;
                // set image
                miniMapBoarder.color = d;
                Debug.Log("change");
            }
          
        }

        if(val == 3)
        {
            // counts throuhg list array
            for (int i = 0; i < images.Length; i++)
            {
                // sets colour for images
                images[i].color = Color.blue;
                // sets colour value
                Color c = scoreBox.color;
                // alpha of image
                c.a = 0.5f;
                // set image
                scoreBox.color = c;
                // sets colour value
                Color d = miniMapBoarder.color;
                // alpha of image
                d.a = 0.5f;
                // set image
                miniMapBoarder.color = d;
                Debug.Log("change");
            }
          
        }
        if (val == 4)
        {
            // counts throuhg list array
            for (int i = 0; i < images.Length; i++)
            {
                // sets colour for images
                images[i].color = Color.yellow;
                // sets colour value
                Color c = scoreBox.color;
                // alpha of image
                c.a = 0.5f;
                // set image
                scoreBox.color = c;
                // sets colour value
                Color d = miniMapBoarder.color;
                // alpha of image
                d.a = 0.5f;
                // set image
                miniMapBoarder.color = d;
                Debug.Log("change");

            }
          
        }
        if (val == 5)
        {
            // counts throuhg list array
            for (int i = 0; i < images.Length; i++)
            {
                // sets colour for images
                images[i].color = Color.magenta;
                // sets colour value
                Color c = scoreBox.color;
                // alpha of image
                c.a = 0.5f;
                // set image
                scoreBox.color = c;
                // sets colour value
                Color d = miniMapBoarder.color;
                // alpha of image
                d.a = 0.5f;
                // set image
                miniMapBoarder.color = d;
                Debug.Log("change");
            }
          
        }
    }
    #endregion
    #region UI Changer
    /// <summary>
    /// updates all images in game on selection. matches up value to dropdown list value, count's through lists and applys to all images and text
    /// </summary>
    /// <param name="val"></param>
    public void UIChangerText(int val)
    {
        // checks value select from 0-5
        if (val == 0)
        {
            // sets colour of all of the images 
            for (int i = 0; i < Textwhite.Length; i++)
            {
                Textwhite[i].color = Color.white;
            }
            for (int i = 0; i < TextsGreen.Length; i++)
            {
                TextsGreen[i].color = Color.green;
            }

            Debug.Log("Default");
        }
        if (val == 1)
        {
            // counts throuhg list array
            for (int i = 0; i < Texts.Length; i++)
            {
                // sets colour for texts
                Texts[i].color = Color.red;
                Debug.Log("change");
            }
        }
        if (val == 2)
        {
            // counts throuhg list array
            for (int i = 0; i < Texts.Length; i++)
            {
                // sets colour for texts
                Texts[i].color = Color.green;
                Debug.Log("change");
            }
        }

        if (val == 3)
        {
            // counts throuhg list array
            for (int i = 0; i < Texts.Length; i++)
            {
                // sets colour for texts
                Texts[i].color = Color.blue;
                Debug.Log("change");
            }
        }
        if (val == 4)
        {
            // counts throuhg list array
            for (int i = 0; i < Texts.Length; i++)
            {
                // sets colour for texts
                Texts[i].color = Color.yellow;
                Debug.Log("change");
            }
        }
        if (val == 5)
        {
            // counts throuhg list array
            for (int i = 0; i < Texts.Length; i++)
            {
                // sets colour for texts
                Texts[i].color = Color.magenta;
                Debug.Log("change");
            }
        }
    }
    #endregion


  
}
