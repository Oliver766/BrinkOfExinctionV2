using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Upgrades : MonoBehaviour
{
    private static int pointsAvailable;
    private static int pointsSpent;

    public TextMeshProUGUI description;
    public TextMeshProUGUI pointsText;

    public GameObject selectRing;

    public List<UpgradeSO> upgrades = new List<UpgradeSO>();
    public Transform upgradeButtons;

    public WeaponsSystems weaponsSystems;

    private static List<int> selectedUpgrades = new List<int>();

    private void Awake()
    {
        // Load any existing upgrades and display this through the UI
        pointsText.text = "Upgrade Points: " + pointsAvailable.ToString();
        for (int i = 0; i < upgradeButtons.childCount; i++)
        {
            Image image = upgradeButtons.GetChild(i).GetComponent<Image>();
            Button button = upgradeButtons.GetChild(i).GetComponent<Button>();

            if (selectedUpgrades.Contains(i))
            {
                image.color = Color.white;
                button.interactable = false;
                upgrades[i].ApplyUpgrade(weaponsSystems);
            }
        }
    }

    /// <summary>
    /// Attempt to credit the player with an upgrade point when the complete a level (Up to a maximum of 3)
    /// </summary>
    /// <returns></returns>
    public bool AddPoints()
    {
        if (pointsSpent < 3)
        {
            pointsAvailable++;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Display the correct UI when the player hovers their mouse over each upgrade
    /// </summary>
    /// <param name="selection">The upgrade the player is hovering over</param>
    public void HoverUpgrade(int selection)
    {
        selectRing.SetActive(true);
        
        RectTransform rectTransform = selectRing.GetComponent<RectTransform>();

        rectTransform.position = upgradeButtons.GetChild(selection).transform.position;

        description.text = upgrades[selection].description;
    }

    /// <summary>
    /// Clear the description and update the UI when the player isn't hovering over anything
    /// </summary>
    public void ClearDescription()
    {
        description.text = string.Empty;
        selectRing.SetActive(false);
    }

    /// <summary>
    /// Attempt to apply the correct upgrade when the player selects it, given they have enough points
    /// </summary>
    /// <param name="selection">The upgrade selected</param>
    public void SelectUpgrade(int selection)
    {
        if(pointsAvailable > 0)
        {
            AudioManager.instance.Play("UI Click");
            AudioManager.instance.Play("Upgrade");

            Image image = upgradeButtons.GetChild(selection).GetComponent<Image>();
            Button button = upgradeButtons.GetChild(selection).GetComponent<Button>();

            button.interactable = false;
            image.color = Color.white;

            pointsAvailable--;
            pointsSpent++;
            pointsText.text = "Upgrade Points: " + pointsAvailable.ToString();

            upgrades[selection].ApplyUpgrade(weaponsSystems);
            selectedUpgrades.Add(selection);
        }
        else
        {
            AudioManager.instance.Play("CoolDown");
        }
    }
}
