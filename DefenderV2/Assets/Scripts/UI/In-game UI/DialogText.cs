using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script by Matt

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog")]
public class DialogText : ScriptableObject
{
    #region Variables
    /// <summary>
    /// Array for paragraphs
    /// </summary>
    [Header("Paragraph")]
    public string[] paragraphs;
    #endregion
}
