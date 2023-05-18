using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854

public class HumanMarker : MonoBehaviour
{
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //Ensure the marker is facing the player

        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.rotation = transform.rotation * Quaternion.Euler(90, 0, 0);

        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);

        // Fade out the marker as the player approaches
        if(distance < 100)
        {
            mat.SetFloat("Alpha", Mathf.Clamp((distance - 10) / 100, 0, 1));
        }
    }
}
