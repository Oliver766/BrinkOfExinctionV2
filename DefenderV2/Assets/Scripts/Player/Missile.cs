using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script by Matthew Harris
// SID 1808854
public class Missile : MonoBehaviour
{
    public Transform target;
    public GameObject explosionPrefab;
    private AudioManager audioManager;

    Vector3 direction;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        audioManager.Play("MissileLaunch");

        // Destroy after 10 seconds (if missile doesn't hit target)

        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * 20 * Time.deltaTime);

        // If missile has a target, aim missile in that direction

        if (target != null)
        {
            direction = target.position - transform.position;

            direction = direction.normalized;

            var rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        // Check if the missile collided with an enemy

        if (enemy)
        {
            audioManager.Play("MissileImpact");

            GameObject explosion = Instantiate(explosionPrefab, enemy.transform.position, enemy.transform.rotation, enemy.transform.parent);

            Destroy(explosion, 1f);
            Destroy(gameObject);

            enemy.TakeDamage(enemy.health);
        }
    }
}
