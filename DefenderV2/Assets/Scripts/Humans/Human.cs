//Last edited: 15/12/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Entity
{
    //INDICATOR
    [SerializeField] GameObject indicator;

    //Vars
    public bool captured = false;
    public bool hasBeenPickedUp = false;
    public bool targeted = false;

    public Rigidbody rb;
    public Collider myCol;
    public Collider myTrigger;

    [SerializeField] List<Human> humans = new List<Human>();
    public Capturer capToFollow;
    private PlayerCollision playerToFollow;

    /// <summary>
    /// Check if this Human should increase or decrease the count of alive humans in the Director.
    /// i.e. call this with "true" when capturing to make sure the value only changes once when capturing.
    /// </summary>
    /// <param name="capturedState">Bool representing if the state of being captured or uncaptured disallows value changes.</param>
    /// <returns>A bool declaring if a value change should happen.</returns>
    bool CanChangeDirectorValOnCapture(bool capturedState)
    {
        //If the Director doesn't exist, exit out immediately
        if(Director.Instance == null) return false;
        //Otherwise go through dupes, assuming changing the Director's aliveHumans is possible.
        bool canChangeVal = true;
        foreach (Human h in humans)
        {
            //Go through each clone to check if already in the captured process
            if (h.captured == capturedState)
            {
                //If a clone is already in the state, break out
                canChangeVal = false;
                break;
            }
        }
        //Return val
        return canChangeVal;
    }

    /// <summary>
    /// Check if this Human should decrease humansAlive in the Director on death.
    /// </summary>
    /// <returns>A bool declaring if a value change should happen.</returns>
    bool CanChangeDirectorValOnDeath()
    {
        //If the Director does not exist, return false.
        if (Director.Instance == null) return false;
        //Otherwise go through dupes, assuming changing the Director's aliveHumans is possible.
        bool canChangeVal = true;
        foreach (Human h in humans)
        {
            //Go through each clone to check if another is already dead
            if (!h.alive)
            {
                //If a clone is already in the state, break out
                canChangeVal = false;
                break;
            }
        }
        return canChangeVal;
    }

    /// <summary>
    /// Capture the human and prevent duplicates from being picked up.
    /// </summary>
    public void Capture()
    {
        //Check playerToFollow status
        if (playerToFollow == null)
        {
            //Call if there's no player to follow (i.e. captured by a Capturer) - decrement alive humans
            if (CanChangeDirectorValOnCapture(true)) Director.Instance.aliveHumans--;
        }
        else
        {
            //Deparent and disable self and dupes
            transform.parent = null;
            foreach(Human h in humans)
            {
                //Go through dupes
                h.transform.parent = null;
                h.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }

        //Set capture info to true and disable gravity
        captured = true;
        rb.useGravity = false;
        hasBeenPickedUp = true;
        myCol.enabled = false;
        myTrigger.enabled = false;
        foreach (Human h in humans)
        {
            //Do the same for the dupes
            h.captured = true;
            h.rb.useGravity = false;
            h.hasBeenPickedUp = true;
            h.myCol.enabled = false;
            h.myTrigger.enabled = false;
        }
    }

    /// <summary>
    /// Call when the human is saved. Dupe position over to friends.
    /// </summary>
    public void UnCapture()
    {
        //If able to change director amounts, increment aliveHumans
        if (CanChangeDirectorValOnCapture(false)) Director.Instance.aliveHumans++;

        //Set capture info to false and re-enable physics
        captured = false;
        rb.useGravity = true;
        myCol.enabled = true;
        myTrigger.enabled = true;
        foreach (Human h in humans)
        {
            //Do the same for the dupes
            h.captured = false;
            h.rb.useGravity = true;
            h.myCol.enabled = true;
            h.myTrigger.enabled = true;
            //Copy over position
            h.transform.localPosition = transform.localPosition;
        }
    }

    /// <summary>
    /// Call at the very start of the program.
    /// </summary>
    void Awake()
    {
        //Get duplicate humans.
        idNum = transform.GetSiblingIndex();
    }

    /// <summary>
    /// Call at the start of the program.
    /// </summary>
    void Start()
    {
        //Set indicator height
        indicator.transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        //Get components
        rb = GetComponent<Rigidbody>();
        myCol = GetComponent<Collider>();
        //Get duplicate humans.
        GetTwins();
    }

    /// <summary>
    /// Call once per frame.
    /// </summary>
    void Update()
    {
        if (captured)
        {
            //If captured, follow the object that captured it.
            if(capToFollow != null)transform.position = capToFollow.capturePos.position;

            if(playerToFollow != null) transform.position = new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y - 1f, playerToFollow.transform.position.z);
        }
    }

    /// <summary>
    /// Call upon collision.
    /// </summary>
    /// <param name="collision">the collision of the object.</param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Water" || hasBeenPickedUp)
        {
            //If the human has been dropped in the water, kill it
            if (CanChangeDirectorValOnDeath()) Director.Instance.aliveHumans--;
            Kill();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerCollision player = other.GetComponent<PlayerCollision>();

        if (player)
        {
            if(player.CaptureHuman())
            {
                playerToFollow = player;
                Capture();
            }
        }
    }

    /// <summary>
    /// Call when this GameObject is destroyed
    /// </summary>
    private void OnDestroy()
    {
        //Check alive bool
        if (!alive)
        {
            //If this has been killed, destroy duplicates.
            foreach (Human h in humans) Destroy(h);
        }
        //Destroy attached GameObject
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Get duplicate humans to synchronise variables.
    /// </summary>
    public void GetTwins()
    {
        //Get terrain
        Transform myTile = transform.parent.parent;
        Transform terrain = myTile.parent;
        for (int i = 0; i < terrain.childCount; i++)
        {
            //Get tile
            Transform currentTransform = terrain.GetChild(i).GetComponent<Transform>();
            //Go through each tile - if it's the tile this human is on, continue
            if (currentTransform == myTile) continue;
            else
            {
                //If not this tile, scan for humans
                Human[] tileHumans = currentTransform.GetChild(1).GetComponentsInChildren<Human>();
                //Add child
                humans.Add(tileHumans[idNum]);
            }
        }
    }

    /// <summary>
    /// Call when enabled.
    /// </summary>
    private void OnEnable()
    {
        foreach(Human h in humans)
        {
            //Go through the List of humans
            if(transform.parent != null)
            {
                //If there's a parent, check the lead tile
                if (h.transform.parent.parent.gameObject.activeSelf)
                {
                    //If a human is on an active tile, copy values over to this
                    captured = h.captured;
                    hasBeenPickedUp = h.hasBeenPickedUp;
                    rb.useGravity = h.rb.useGravity;
                    myCol.enabled = h.myCol.enabled;
                }
            }
        }
    }
}
