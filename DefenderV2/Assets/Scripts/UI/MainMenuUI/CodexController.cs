using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Script by Matthew Harris
// SID 1808854
public class CodexController : MonoBehaviour
{

    public GameObject selectionRing;     
    public Transform[] options;
    public GameObject[] optionNames;

    public Animator iconAnim;
    public Animator scanAnim;

    public GameObject mainScreen;
    public GameObject optionsScreen;
    public GameObject upgradesScreen;
    public GameObject creditsScreen;

    public GameController controller;

    /// <summary>
    /// Show relevent UI when the player hovers over a button with their mouse
    /// </summary>
    /// <param name="selection"></param>
    public void HoverOption(int selection)
    {
        selectionRing.SetActive(true);
        AudioManager.instance.Play("UI Hover");
        selectionRing.GetComponent<RectTransform>().position = options[selection].GetComponent<RectTransform>().position;

        for (int i = 0; i < optionNames.Length; i++)
        {
            optionNames[i].SetActive(i == selection);
        }

        iconAnim.SetFloat("Selection", selection);
    }

    /// <summary>
    /// Hide the relevent UI when the player isn't hovering over anything
    /// </summary>
    public void Deselect()
    {
        selectionRing.SetActive(false);
        iconAnim.SetFloat("Selection", -1);

        for (int i = 0; i < optionNames.Length; i++)
        {
            optionNames[i].SetActive(false);
        }
    }

    /// <summary>
    /// Show options screen
    /// </summary>
    public void Options()
    {
        Invoke(nameof(MainToOptions), 0.5f);
        scanAnim.Play("ScanScreen");
    }

    /// <summary>
    /// Show upgrade screen
    /// </summary>
    public void Upgrades()
    {
        Invoke(nameof(MainToUpgrades), 0.5f);
        scanAnim.Play("ScanScreen");
    }

    /// <summary>
    /// Show credits screen
    /// </summary>
    public void Credits()
    {
        Invoke(nameof(MainToCredits), 0.5f);
        scanAnim.Play("ScanScreen");
    }

    /// <summary>
    /// Return to main screen
    /// </summary>
    public void Back()
    {
        Invoke(nameof(BackToMain), 0.5f);
        scanAnim.Play("ScanScreen");
    }

    /// <summary>
    /// Return to ship and main menu
    /// </summary>
    public void BackToShip()
    {
        Invoke(nameof(ClearScreen), 0.5f);
        scanAnim.Play("ScanScreen");
    }

    /// <summary>
    /// Display the screen when transitioning from the main menu
    /// </summary>
    public void OpenDisplay()
    {
        Invoke(nameof(ShowScreen), 0.5f);
        scanAnim.Play("ScanScreen");
    }

    /// <summary>
    /// Set the screen to active
    /// </summary>
    private void ShowScreen()
    {
        mainScreen.SetActive(true);
    }

    /// <summary>
    /// Clear the screen
    /// </summary>
    private void ClearScreen()
    {
        mainScreen.SetActive(false);
        controller.ShowMenu();
    }

    /// <summary>
    /// Transition from main screen to options
    /// </summary>
    private void MainToOptions()
    {
        mainScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    /// <summary>
    /// Transition from main screen to upgrades
    /// </summary>
    private void MainToUpgrades()
    {
        mainScreen.SetActive(false);
        upgradesScreen.SetActive(true);
    }

    /// <summary>
    /// Transition from main screen to credits
    /// </summary>
    private void MainToCredits()
    {
        mainScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    /// <summary>
    /// Transition from all screens to main
    /// </summary>
    private void BackToMain()
    {
        mainScreen.SetActive(true);
        upgradesScreen.SetActive(false);
        optionsScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

}
