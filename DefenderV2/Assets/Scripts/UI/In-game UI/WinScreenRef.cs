//Last edited: 28/12/21
//Auhtor: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenRef : MonoBehaviour
{
    public GameObject upgradeText;
    public Upgrades upgrades;
    /// <summary>
    /// Call when the scene starts.
    /// </summary>
    void Start()
    {
        //Find Director
        if (Director.Instance != null)
        {
            //If the Director exists, set its win screen reference to this.
            Director.Instance.winScreen = this;         
            Debug.Log("REF'D. Win screen works.");
        }
        //DEBUG - If there's no Director to reference, warn in a Debug Log.
        else Debug.LogError("WARNING: NO DIRECTOR REFERENCE. WIN SCREEN WILL NOT APPEAR ON WIN.");
        //Disable the Win Screen. It will re-enable when the player wins a level.
        gameObject.SetActive(false);
    }
}
