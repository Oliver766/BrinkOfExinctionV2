using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
// script by Oliver Lancashire
// SID 190981  

public class ButtonShine : MonoBehaviour
{
    #region Variables
    [Header("Variables")]
    /// <summary>
    /// reference for shine image
    /// </summary>
    public Transform shine;
    /// <summary>
    /// offset of image
    /// </summary>
    public float offset;
    /// <summary>
    /// speed of image
    /// </summary>
    public float speed;
    /// <summary>
    ///  min delay of movement
    /// </summary>
    public float minDelay;
    /// <summary>
    /// max delay of movement
    /// </summary>
    public float maxDelay;
    #endregion
    #region Start
    void Start()
    {
        // beginm function on start
        Animate();   
    }
    #endregion
    #region Animation
    /// <summary>
    /// animation function
    /// </summary>
    public void Animate()
    {
        // set movement of image
        shine.DOLocalMoveX(offset, speed).SetEase(Ease.Linear).SetDelay(Random.Range(minDelay, maxDelay)).OnComplete(() =>
              {
                  // checks if image has reached the end
                  shine.DOLocalMoveX(-offset, 0);
                  // run function again
                  Animate();
              });
    }
    #endregion

}
