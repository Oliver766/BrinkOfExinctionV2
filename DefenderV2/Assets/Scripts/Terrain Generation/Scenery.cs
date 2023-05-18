using UnityEngine;

[CreateAssetMenu(fileName = "New Scenery", menuName = "Scenery")]
public class Scenery : ScriptableObject
{
    public GameObject[] models;

    public int amount;
    public float density;

    public float maxHeight;
    public float minHeight;

    public bool alignToSurface;
}
