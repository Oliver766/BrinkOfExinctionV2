//Last Edited: 29/12/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    //Vars
    string shipName;
    public int health;
    public int defence;

    //Behavioural states
    public enum EnemyState { Move, Action, Return };
    public EnemyState myState = EnemyState.Move;
    public bool finishedJob = false;

    //A list of duplicate enemies specific to this instance.
    public List<Enemy> enemies = new List<Enemy>();

    //LERPING
    public Vector3[] v3Points;
    public bool canMove = false;
    public Vector3 posToReturnTo = Vector3.zero;
    public bool returnLerp = false;
    public bool recentReturn = false;

    //Base class specific variables
    public bool baseClass = true;
    Vector3 startPos;
    float originalDist = 100f;
    float actionTimer = 0f;
    bool fired = false;
    [SerializeField] Transform firePos;
    [SerializeField] GameObject bullet;

    //FX
    public AudioSource engineSound;
    public AudioSource otherSound;
    //Array of AudioClips for actions - [0] plays first in AI loop, [1] plays after.
    public AudioClip[] fxClips = new AudioClip[2];

    public float speed = 1f;

    private Radar radar;

    #region BEHAVIOURS

    /// <summary>
    /// Use recursion to find the point on the Bézier curve the enemy is supposed to go to
    /// </summary>
    /// <param name="currentPoints">The points being used to make the calculation.</param>
    public virtual void BezierLerp(Vector3[] currentPoints)
    {
        float time = Mathf.PingPong(Time.time * speed, 1);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        //Calculate lerp fraction variable
        if (currentPoints.Length == 2 || recentReturn)
        {
            //If there are only two points remaining, just lerp between them.
            transform.localPosition = Vector3.Lerp(currentPoints[0], currentPoints[currentPoints.Length - 1], time);
        }
        else
        {
            //Setup recursion process by creating a new smaller array
            Vector3[] deeperPoints = new Vector3[currentPoints.Length - 1];
            //Create the new points by lerping between the current ones.
            for(int i = 0; i < currentPoints.Length - 1; i++) deeperPoints[i] = Vector3.Lerp(currentPoints[i], currentPoints[i + 1], time * speed);
            //RECURSE
            BezierLerp(deeperPoints);
        }
    }

    /// <summary>
    /// Check if it is possible to perform an action. Usually only specific to base class.
    /// </summary>
    public void ActionCheck()
    {
        //Check raycast between here and v3.zero - if blocked, return
        RaycastHit hit;
        Vector3 aircraftPos = CharacterControl.instance.transform.position;
        if(Physics.Linecast(transform.position, aircraftPos, out hit))
        {
            //Something is in the way between the player and enemy - if it's terrain, quit out the function.
            if (hit.collider.tag == "Terrain") return;
        }

        //If already fired, return
        if (finishedJob) return;

        //If still standing, let action take place.
        Debug.Log("READY TO FIRE");
        transform.LookAt(new Vector3(aircraftPos.x, transform.position.y, aircraftPos.z));
        posToReturnTo = transform.localPosition;
        startPos = transform.position;
        originalDist = Vector3.Distance(Vector3.zero, transform.position);
        actionTimer = 0f;
        otherSound.PlayOneShot(fxClips[0]);
        myState = EnemyState.Action;
        
        foreach(Enemy e in enemies)
        {
            //Sync variables with duplicates
            e.posToReturnTo = posToReturnTo;
            e.actionTimer = 0f;
            //e.myState = EnemyState.Action; DO NOT GIVE ENEMYSTATE, OTHERWISE THEY GROUP ONTO THE PLAYER
        }
    }

    /// <summary>
    /// Generic action class than can be called as behaviours. Unless overwritten, the standard type will fire at the player.
    /// </summary>
    public virtual void Action()
    {
        //Increment timer
        Vector3 aircraftPos = CharacterControl.instance.transform.position;
        actionTimer += Time.deltaTime;
        //If under a second, lerp towards player
        if (actionTimer < 1f) transform.position = Vector3.Lerp(startPos, aircraftPos, actionTimer / 4);
        //Else if under 1.5f, look at player and wait to fire
        else if (actionTimer < 1.5f) transform.LookAt(new Vector3(0, aircraftPos.y, 0));
        else
        {
            //After 1.5 seconds, check if the gun can fire.
            if (!fired)
            {
                //If it's possible to fire, check if there's a clear shot
                RaycastHit hit;
                if (!Physics.Linecast(transform.position, aircraftPos, out hit))
                {
                    //If there's a clear shot, FIRE
                    Debug.Log("FIRE!!!");
                    Instantiate(bullet, firePos.position, transform.localRotation * Quaternion.Euler(90, 0, 0), null);
                    otherSound.PlayOneShot(fxClips[1]);
                }

                fired = true;
            }
            else
            {
                //Otherwise continue with AI
                if(actionTimer >= 1.75f)
                {
                    //Go to return state
                    finishedJob = true;
                    fired = false;
                    myState = EnemyState.Return;
                    foreach (Enemy e in enemies)
                    {
                        //Sync other enemies so they keep up.
                        e.finishedJob = true; 
                        e.myState = EnemyState.Return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Return to the move state.
    /// </summary>
    /// <param name="currentPoints">The points being used to make the calculaton.</param>
    void ReturnHome(Vector3[] currentPoints)
    {
        if (!returnLerp)
        {
            //Stage one of the return stage - move towards last point on the transform line it was on before Action.
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, posToReturnTo, (speed * 12.5f) * Time.deltaTime);
            //If matching the position it was in before Action, move to the next stage.
            if (transform.localPosition == posToReturnTo) returnLerp = true;
        }
        else
        {
            //Stage two of the return stage - transition back to lerping by finding position to lerp to. 
            float time = Mathf.PingPong(Time.time * speed, 1);
            Vector3 aimedPos = Vector3.Lerp(v3Points[0], v3Points[v3Points.Length - 1], time);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimedPos, (speed * Time.deltaTime) * 25);

            if (transform.localPosition == aimedPos)
            {
                //If matching the lerped position, return to Move.
                myState = EnemyState.Move;
                returnLerp = false;
                recentReturn = true;
                foreach (Enemy e in enemies)
                {
                    //Parse over variables to dupes.
                    e.transform.localPosition = transform.localPosition;
                    e.myState = EnemyState.Move;
                    e.returnLerp = false;
                }
            }
        }

    }

    /// <summary>
    /// Parse damage taken from player.
    /// </summary>
    /// <param name="damage">The int value of the damage taken.</param>
    public void TakeDamage(int damage)
    {
        Debug.Log(damage);
        //Decrease health
        health -= damage;
        //If enough to kill, run Kill() script and show explosion, else parse value to duplicates.
        if (health <= 0)
        {
            Kill();
        }
        else foreach (Enemy e in enemies) e.health -= damage;
    }
    #endregion

    #region ENEMY_SETUP

    /// <summary>
    /// Set position of the enemy, and pass to duplicates.
    /// </summary>
    public void SetPath()
    {
        canMove = true;
        //SET V3s in for loop
        v3Points = new Vector3[Random.Range(2, 4)];
        //Set start and end
        v3Points[0] = transform.localPosition;
        v3Points[v3Points.Length - 1] = new Vector3((transform.localPosition.x + Random.Range(-30, 31)), transform.localPosition.y, (transform.localPosition.z + Random.Range(-30, 31)));
        if (v3Points.Length > 2)
        {
            //If there's 3 points, set a midpoint.
            v3Points[1] = new Vector3(((v3Points[0].x + v3Points[2].x) / 2) + Random.Range(-10, 10),
                                       transform.localPosition.y,
                                       ((v3Points[0].z + v3Points[2].z) / 2) + Random.Range(-10, 10));
        }
        //Change speed based on distance.
        float dist = Vector3.Distance(v3Points[0], v3Points[v3Points.Length - 1]);
        //If above average distance, slow it down speed.
        if (dist >= 21.2f) speed *= 21.2f / dist;
        //If there aren't enough points, warn the system thru the console
        if (v3Points.Length < 2) Debug.LogWarning("WARNING: SHIP @ " + transform.position + " WILL NOT LERP CORRECTLY");
        //Get child index for future destruction
        CheckMesh();
        GetTwins();
        //Set for duplicates
        foreach (Enemy e in enemies)
        {
            //Copy and paste values, then disable the dupes.
            e.v3Points = v3Points;
            e.canMove = true;
            e.GetTwins();
            e.speed = speed;
        }
    }

    /// <summary>
    /// Check the destination points to make sure that the enemy's waypoints aren't blocked by mountains.
    /// </summary>
    void CheckMesh()
    {
        //Wait a bit so the mesh can spawn first.
        RaycastHit hit;
        //Go through each point
        for (int i = 0; i < v3Points.Length; i++)
        {
            //Raycast from directly above the point
            Ray ray = new Ray(new Vector3(v3Points[i].x, 100f, v3Points[i].z), -Vector3.up);
            if (Physics.Raycast(ray, out hit))
            {
                //If there's a hit, continue
                if (hit.collider.tag == "Terrain")
                {
                    //If the raycast involves the mountains, spawn the waypoint above the mountains.
                    v3Points[i] = new Vector3(hit.point.x, hit.point.y + 2.5f, hit.point.z);
                }
            }
            //Reset position so it's not in the earth.
            transform.localPosition = v3Points[0];
        }

        //Constantly check if something's in the way until it isn't
        bool aboveGround = false;
        do
        {
            //Check in between major points
            if (Physics.Linecast(v3Points[0], v3Points[1], out hit))
            {
                //Check if anything in the way between two points
                if (hit.transform.tag == "Terrain")
                {
                    //If so, just send everything higher
                    for (int i = 0; i < v3Points.Length; i++) v3Points[i] = new Vector3(v3Points[i].x, v3Points[i].y + 2f, v3Points[i].z);
                }
                else aboveGround = true;
            }
            else
            {
                //If not, break loop
                aboveGround = true;
            }
        } while (!aboveGround);

    }

    /// <summary>
    /// Enable the nearest duplicate.
    /// </summary>
    void EnableNearest()
    {
        //Go through dupes
        foreach(Enemy e in enemies)
        {
            if (!e.enabled)
            {
                //If it's disabled, check its distance from 0,0,0
                if (Vector2.Distance(e.transform.position, Vector2.zero) < 300)
                {
                    //If it's less than 50, enable it.
                    e.enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Get reference to duplicate enemies.
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
            //Go through each tile - if it's the tile this enemy is on, continue
            if (currentTransform == myTile) continue;
            else
            {
                //If not this tile, scan for enemies
                Enemy[] tileEnemies = currentTransform.GetChild(0).GetComponentsInChildren<Enemy>();
                //Add child
                enemies.Add(tileEnemies[idNum]);
            }
        }
    }

    /// <summary>
    /// Check if this Human should decrease enemiesLeft in the Director on death.
    /// </summary>
    /// <returns>A bool declaring if a value change should happen.</returns>
    public bool CanChangeDirectorValOnDeath()
    {
        //If the Director does not exist, return false.
        if (Director.Instance == null) return false;

        //Otherwise go through dupes, assuming changing the Director's enemiesLeft is possible.
        bool canChangeVal = true;
        foreach (Enemy e in enemies)
        {
            //Go through each clone to check if another is already dead
            if (!e.alive)
            {
                //If a clone is already in the state, break out
                canChangeVal = false;
                break;
            }
        }
        return canChangeVal;
    }
    #endregion

    /// <summary>
    /// Call at start of program
    /// </summary>
    void Awake()
    {
        //Get duplicate enemies.
        idNum = transform.GetSiblingIndex();
        //Get audioSources if null
        if (engineSound == null) engineSound = GetComponent<AudioSource>();
        if (otherSound == null) otherSound = GetComponent<AudioSource>();

        radar = FindObjectOfType<Radar>();
        radar.AddEnemy(transform);
    }

    /// <summary>
    /// Call once per frame
    /// </summary>
    void Update()
    {
        //Check if the CharacterControl instance exists
        if(CharacterControl.instance != null)
        {
            //If the player cannot control their ship, exit out - if not, make sure ship noise is playing.
            if (!CharacterControl.instance.active) return;
            else if (!engineSound.isPlaying && gameObject.activeSelf) engineSound.Play();
        }
        //If not alive, return immediately and kill this enemy.
        if (!alive) Kill();
        //Check position
        if (Vector3.Distance(Vector3.zero, transform.position) >= 300)
        {
            //If this enemy is too far away from 0,0,0, call EnableNearest.
            EnableNearest();
            this.enabled = false;
        }
        if (canMove)
        {
            //If the enemy can move, act based on state.
            switch (myState)
            {
                case EnemyState.Move:
                    //Calculate position in Bézier curve
                    BezierLerp(v3Points);
                    if (baseClass)
                    {
                        //If the base class, reset finishedJob where necessary and check if Action can take place.
                        if (Vector3.Distance(transform.localPosition, v3Points[0]) < 1f)
                        {
                            finishedJob = false;
                            recentReturn = false;
                        }
                        if (Vector3.Distance(Vector3.zero, transform.position) <= 30 && !recentReturn) ActionCheck();
                    }
                    break;
                case EnemyState.Action:
                    //Perform an action, by default, fire at the player
                    if (!finishedJob) Action();
                    else myState = EnemyState.Return;
                    break;
                case EnemyState.Return:
                    //Go back to moving.
                    ReturnHome(v3Points);
                    break;
                default:
                    Debug.LogWarning("ENEMY OUT OF STATE.");
                    break;
            }
        }
        
    }

    /// <summary>
    /// Call when the gameObject is destroyed
    /// </summary>
    private void OnDestroy()
    {
        //Stop engine audio
        engineSound.Stop();
        //Check alive bool
        if (!alive)
        {
            //If killed, kill duplicates
            if (CanChangeDirectorValOnDeath()) Director.Instance.enemiesLeft--;
            if (Director.Instance.enemiesLeft <= 0) Director.Instance.FinishLevel();
            else Director.Instance.playerScore += score;
            foreach (Enemy e in enemies) Destroy(e);
        }
        //Destroy attached gameObject
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Call when the GameObject is enabled.
    /// </summary>
    private void OnEnable()
    {
        //Play engine audio if player can control their ship.
        if(CharacterControl.instance != null)
        {
            if(CharacterControl.instance.active) engineSound.Play();
        }

        //Find the nearest active tile.
        foreach(Enemy e in enemies)
        {
            //Go through each of the enemy duplicates
            if (e.transform.parent.parent.gameObject.activeSelf)
            {
                //If the parent is active, copy its variables to synchronize.
                transform.localPosition = e.transform.localPosition;
                myState = e.myState;
                finishedJob = e.finishedJob;
                break;
            }
        }
    }
}
