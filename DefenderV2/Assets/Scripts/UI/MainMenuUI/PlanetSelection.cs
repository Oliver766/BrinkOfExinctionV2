using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetSelection : MonoBehaviour
{
    public Image[] planetImages;
    public GameObject planetSelectionRing;
    public GameObject[] planetNames;

    public GameController controller;

    private int level;

    /// <summary>
    /// Display the correct UI when the player hovers over a planet image with their mouse
    /// </summary>
    /// <param name="selection"></param>
    public void HoverPlanet(int selection)
    {
        if (selection <= level)
        {
            planetSelectionRing.SetActive(true);
            AudioManager.instance.Play("UI Hover");
            planetSelectionRing.GetComponent<RectTransform>().position = planetImages[selection].GetComponent<RectTransform>().position;

            for (int i = 0; i < planetNames.Length; i++)
            {
                planetNames[i].SetActive(i == selection);
            }
        }
    }

    /// <summary>
    /// Display the correct UI when the player is no longer hovering over a planet image
    /// </summary>
    public void DeselectPlanet()
    {
        planetSelectionRing.SetActive(false);

        for (int i = 0; i < planetNames.Length; i++)
        {
            planetNames[i].SetActive(false);
        }
    }

    /// <summary>
    /// Proceed to the next step in loading the planet the player has selected
    /// </summary>
    /// <param name="selection">The planet/level selected by the player</param>
    public void SelectPlanet(int selection)
    {
        if(selection <= level)
        {
            AudioManager.instance.Play("UI Click");
            GetComponent<Animator>().Play("HidePlanetSelection");
            controller.planetLoading(selection);
        }
    }

    /// <summary>
    /// Hide any planets which have not yet been reached by the player
    /// </summary>
    /// <param name="level">The current level/planet the player is on</param>
    public void DisplayPlanets(int level)
    {
        GetComponent<Animator>().Play("ShowPlanetSelection");

        this.level = level;

        for (int i = 0; i < planetImages.Length; i++)
        {
            if (i > level)
            {
                planetImages[i].color = new Color(0.3f, 0.3f, 0.3f, 1f);
            }
            else
            {
                planetImages[i].color = Color.white;
            }
        }
    }

    /// <summary>
    /// Return to the main menu
    /// </summary>
    public void Back()
    {
        GetComponent<Animator>().Play("HidePlanetSelection");
        controller.ShowMenu();
    }
}
