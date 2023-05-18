using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Settings", menuName = "Terrain Settings")]
public class TerrainSettings : ScriptableObject
{
    public float heightModifier = 5;
    public float edgeFalloff = 15;
    public float roughness = 3;
    public float islandSize = 0.1f;
    public int islandHeight = 10;
    public float maxHeight = 5;
    public float minHeight = -10;

    public Gradient gradient;

    public Scenery[] scenery;
}
