using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]

public class DisableShader : MonoBehaviour
{
    public Material[] materials;

    public bool enable = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Material mat in materials)
        {
            mat.SetInt("ENABLE_BENDING", 1);
        }
    }

    void OnApplicationQuit()
    {
        foreach (Material mat in materials)
        {
            mat.SetInt("ENABLE_BENDING", 0);
        }
    }

    private void Update()
    {
        if (enable)
        {
            foreach (Material mat in materials)
            {
                mat.SetInt("ENABLE_BENDING", 1);
            }
        }
        else
        {
            foreach (Material mat in materials)
            {
                mat.SetInt("ENABLE_BENDING", 0);
            }
        }
    }
}
