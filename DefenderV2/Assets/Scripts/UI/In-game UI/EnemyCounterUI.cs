//Last edited: 09/12/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCounterUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI enemyCount;

    /// <summary>
    /// Update every frame.
    /// </summary>
    void Update()
    {
        //Act if there's a director - if not, just disable this display.
        if (Director.Instance != null)
        {
            //If there's a director, set the enemies left display to show the number of enemies left. Can have animation implemented.
            enemyCount.text = Director.Instance.enemiesLeft.ToString();
        }
        else gameObject.SetActive(false);
    }
}
