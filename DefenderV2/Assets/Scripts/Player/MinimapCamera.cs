using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854

public class MinimapCamera : MonoBehaviour
{
    public Transform aircraft;

    // Update is called once per frame
    void Update()
    {
        // Follow the rotation of the player (Can't be fixed parented as only needs to follow one rotation)
        transform.rotation = Quaternion.Euler(90, 0, -aircraft.rotation.eulerAngles.y + 180);
    }
}
