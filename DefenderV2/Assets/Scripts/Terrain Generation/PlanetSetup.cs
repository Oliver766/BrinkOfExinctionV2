using System.Collections;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854

public class PlanetSetup : MonoBehaviour
{
    public PlanetSettings[] planets;

    public TerrainGenerator terrain;
    public Renderer ocean;
    public Renderer sky;
    public Light sun;

    /// <summary>
    /// Load the terrain settings and
    /// </summary>
    /// <param name="selection">The planet selection</param>
    public void SelectPlanet(int selection)
    {
        // Set the ocean and sky materials
        ocean.material = planets[selection].oceanMaterial;
        sky.material = planets[selection].skyboxMaterial;

        AudioManager.instance.CancelAllPlayWithDelay();
        AudioManager.instance.FadeOut("Menu Intro", 5f);
        AudioManager.instance.FadeOut("Menu Loop", 5f);

        //// Fade out the menu music
        //if (AudioManager.instance.IsSoundPlaying("Menu Intro"))
        //{
        //    AudioManager.instance.FadeOut("Menu Intro", 5f);
        //    AudioManager.instance.CancelPlayWithDelay("Menu Loop");
        //}
        //else
        //{
        //    AudioManager.instance.FadeOut("Menu Loop", 5f);
        //}

        // Fade into the relevent soundtrack for each planet
        AudioManager.instance.Play("Level " + (selection + 1) + " Intro");
        AudioManager.instance.PlayWithDelay("Level " + (selection + 1) + " Loop", AudioManager.instance.GetSoundLength("Level " + (selection + 1) + " Intro"));

        StartCoroutine(LerpSunColour(selection));

        // Generate the terrain for the selected planet
        terrain.GenerateTerrain(planets[selection].terrainSettings);
    }

    /// <summary>
    /// Lerp the colour of the lighting
    /// </summary>
    /// <param name="selection"></param>
    /// <returns></returns>
    private IEnumerator LerpSunColour(int selection)
    {
        yield return new WaitForSeconds(11f);

        for (int i = 0; i < 100; i++)
        {
            sun.color = Color.Lerp(Color.white, planets[selection].lightingColor, i / 100f);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
