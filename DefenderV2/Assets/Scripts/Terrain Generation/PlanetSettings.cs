using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Planet Settings", menuName = "Planet Settings")]
public class PlanetSettings : ScriptableObject
{
    public TerrainSettings terrainSettings;
    public Material oceanMaterial;
    public Material skyboxMaterial;
    public Color lightingColor;
}
