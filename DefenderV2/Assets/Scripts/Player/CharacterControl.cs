using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

// Script by Matthew Harris
// SID 1808854

public class CharacterControl : MonoBehaviour
{
    public Transform terrain;
    public Animator meshAnimator;
    public Animator HUDAnimator;

    public PlayerCollision playerCollision;
    public HUDController HUD;

    public float turnSpeed = 7f;
    public float throttle = 7f;
    public float strafeSpeed = 5f;

    public bool active = false;

    public static CharacterControl instance;

    public ParticleSystem trailLeft;
    public ParticleSystem trailRight;
    public ParticleSystem tractorBeam;

    public RectTransform crossHair;

    public TextMeshProUGUI altitude;

    public CinemachineVirtualCamera aircraftCam;

    private Vector2 lookInput, screenCenter, mouseDistance;

    private WeaponsSystems weaponsSystems;

    private void Awake()
    {
        instance = this;
        weaponsSystems = GetComponent<WeaponsSystems>();
    }

    private void Start()
    {
        screenCenter.x = Screen.width / 2;
        screenCenter.y = Screen.height / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Display the altitude of the player
        altitude.text = ((int)(transform.position.y * 100) - 130).ToString();

        if (active)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            lookInput.x = Input.mousePosition.x;
            lookInput.y = Input.mousePosition.y;

            // Calculate the mouse screen position
            mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.x;
            mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;


            if(transform.position.y > 15)
            {
                mouseDistance.y = Mathf.Clamp(mouseDistance.y, 0, 100);
            }

            // Move the crosshair with the mouse (to a maximum amount inside the centre HUD)
            crossHair.localPosition = new Vector3(mouseDistance.x * 90, -mouseDistance.y * 70, 0);

            transform.Rotate(0f, mouseDistance.x * 60f * Time.deltaTime, 0f, Space.Self);
            transform.Translate(0, -mouseDistance.y * 2f * Time.deltaTime, 0);

            meshAnimator.SetFloat("Y", -mouseDistance.y * 2, 0.5f, Time.deltaTime);
            meshAnimator.SetFloat("X", mouseDistance.x * 2, 0.5f, Time.deltaTime);

            if (mouseDistance.x > 0.5f || mouseDistance.x < -0.5f)
            {
                trailLeft.Play();
                trailRight.Play();
            }
            else
            {
                trailLeft.Stop();
                trailRight.Stop();
            }

            foreach (Transform tile in terrain)
            {
                // Move tiles in the opposite direction of the player to create the illusion of movement

                tile.Translate(throttle * Time.deltaTime * transform.forward);

                // Disable tiles if they are out of view

                if (Vector2.Distance(new Vector2(tile.position.x + 100, tile.position.z + 100), Vector2.zero) > 200)
                {
                    tile.gameObject.SetActive(false);
                }
                else
                {
                    tile.gameObject.SetActive(true);
                }

                // Move planes from the edge of the map back to the start when they pass a certain point;

                if (tile.localPosition.z > 300)
                {
                    tile.position += new Vector3(0, 0, -600);
                }

                if (tile.localPosition.z < -300)
                {
                    tile.position += new Vector3(0, 0, 600);
                }

                if (tile.localPosition.x > 300)
                {
                    tile.position += new Vector3(-600, 0, 0);
                }

                if (tile.localPosition.x < -300)
                {
                    tile.position += new Vector3(600, 0, 0);
                }
            }

            // Activate the tractor beam
            if (Input.GetMouseButtonDown(1))
            {
                playerCollision.canCapture = true;
                tractorBeam.Play();
                AudioManager.instance.Play("Tractor Beam");
            }

            if (Input.GetMouseButtonUp(1))
            {
                playerCollision.canCapture = false;
                tractorBeam.Stop();
                AudioManager.instance.Stop("Tractor Beam");
            }
        }
    }

    /// <summary>
    /// Enable or disable the controllers, and change the relevant UI elements
    /// </summary>
    /// <param name="enabled">Whether to enable or disable</param>
    public void EnableControls(bool enabled)
    {
        active = enabled;
        weaponsSystems.EnableWeapons(enabled);
        HUDAnimator.SetBool("Enabled", enabled);

        if (enabled)
        {
            HUD.canPause = true;
            Destroy(GetComponent<Animator>());
        }
        else
        {
            HUD.canPause = false;
        }
    }

    /// <summary>
    /// End the game and disable the controls
    /// </summary>
    public void EndGame()
    {
        EnableControls(false);

        meshAnimator.SetBool("EndGame", true);

        // Change the camera to follow the aircraft for the ending sequence
        aircraftCam.LookAt = meshAnimator.transform;
    }
}
