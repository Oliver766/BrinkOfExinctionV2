using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyedShipCleanUp : MonoBehaviour
{
    public GameObject explosionPrefab;
    public Transform smoke;

    /// <summary>
    /// Used for destroying the remaining ship parts once they collide with the ground after the explosion
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Player"))
        {
            Instantiate(explosionPrefab, smoke.position, smoke.rotation, null);

            Destroy(gameObject);
        }
    }
}
