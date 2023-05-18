using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854
public class Radar : MonoBehaviour
{
    public List<Transform> enemyTransforms = new List<Transform>();
    public List<Transform> radarMarkers = new List<Transform>();

    public Material dotMat;
    public Material arrowMat;

    public GameObject radarPrefab;

    /// <summary>
    /// Add an enemy to the radar
    /// </summary>
    /// <param name="enemy">The enemy transform to add</param>
    public void AddEnemy(Transform enemy)
    {
        enemyTransforms.Add(enemy);

        // Create a radar marker to follow the enemy
        GameObject radarMarker = Instantiate(radarPrefab, transform);
        radarMarkers.Add(radarMarker.transform);
    }

    private void Update()
    {
        for (int i = 0; i < enemyTransforms.Count; i++)
        {
            if (enemyTransforms[i] != null)
            {
                float distance = Vector3.Distance(Vector3.zero, enemyTransforms[i].transform.position);

                // If the enemy is still visible on the map, show a dot and follow the enemy directly
                if (distance < 50)
                {
                    radarMarkers[i].gameObject.SetActive(true);

                    radarMarkers[i].GetComponent<Renderer>().material = dotMat;
                    radarMarkers[i].transform.position = enemyTransforms[i].transform.position + Vector3.up * 10;
                }

                // If the enemy is not visible on the map, change the dot to an arrow and simply point in that direction on the edge of the map
                else if(distance < 80)
                {
                    radarMarkers[i].gameObject.SetActive(true);

                    radarMarkers[i].GetComponent<Renderer>().material = arrowMat;
                    radarMarkers[i].transform.position = Vector3.Lerp(Vector3.zero, enemyTransforms[i].position, 50 / distance);
                    radarMarkers[i].transform.position = new Vector3(radarMarkers[i].transform.position.x, 10, radarMarkers[i].transform.position.z);
                        
                    Vector3 lookAtRotation = Quaternion.LookRotation(enemyTransforms[i].position).eulerAngles;
                    radarMarkers[i].rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0, 1, 0)));
                }

                // If the enemy is completely out of range, disable the radar marker
                else
                {
                    radarMarkers[i].gameObject.SetActive(false);
                }
            }

            // If the enemy has been destroyed, destroy the radar marker too
            else
            {
                if(radarMarkers[i] != null)
                {
                    Destroy(radarMarkers[i].gameObject);
                }
            }
        }
    }
}
