using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Script by Matthew Harris
// SID 1808854
public class ButtonSelector : MonoBehaviour
{
    #region Variables
    [Header("Object transforms")]

    public Transform buttons;
    public RectTransform selector;

    public float offset = 50f;

    private float end;
    private float start;
    private float t;
    private bool active = false;
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            t += 3f * Time.deltaTime;
            selector.position = new Vector3(selector.position.x, Mathf.Lerp(start, end, t), selector.position.z);
        }
    }
    #endregion
    #region ChangePosition
    /// <summary>
    /// Move the image to the position of the button minus an offset amount
    /// </summary>
    /// <param name="button"></param>
    public void ChangePosition(int button)
    {
        t = 0;
        start = selector.position.y;
        end = buttons.GetChild(button).GetComponent<RectTransform>().position.y - offset;
        active = true;
    }
    #endregion
}
