//Last edited: 02/12/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreTracker : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI scoreText;

    /// <summary>
    /// Reset the score (used on a game over or switching to the main menu).
    /// </summary>
    public void ResetScore()
    {
        //Throw if director non-existent, otherwise reset score
        if (Director.Instance == null) return;
        Director.Instance.playerScore = 0;
    }

    /// <summary>
    /// Call every frame
    /// </summary>
    void Update()
    {
        if(Director.Instance != null)
        {
            //If there's a director, update the score with the score changes.
            scoreText.text = Director.Instance.playerScore.ToString();
        }
    }
}
