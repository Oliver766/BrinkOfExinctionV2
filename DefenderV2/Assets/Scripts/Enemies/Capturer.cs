//Last edited: 15/12/21
//Author: Aidan McHugh
//SID: 1806867

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capturer : Enemy
{
    //Vars
    public Human myHuman;
    Vector3 aboveHuman;
    public float waitTimer = 0f;
    public Transform capturePos;
    float maxTime = 2.5f;
    //Persistence - a list of duplicate Capturers specific to this instance
    List<Capturer> capturers = new List<Capturer>();

    //Enum
    public bool canPickUp = false;

    public ParticleSystem beam;

    //Override movement to trigger Action() if close to nearest human.
    public override void BezierLerp(Vector3[] currentPoints)
    {
        //Lerp from base class
        base.BezierLerp(currentPoints);
        //If no human, increment timer
        if(!finishedJob) waitTimer += Time.deltaTime;
        if(waitTimer >= maxTime)
        {
            //If the timer has been reached, look at human and start Action().
            //NOTE - ASSUMES IT'S NOT PICKED UP
            transform.LookAt(new Vector3(myHuman.transform.position.x, transform.position.y, myHuman.transform.position.z));
            aboveHuman = new Vector3(myHuman.transform.localPosition.x, myHuman.transform.localPosition.y + 2.5f, myHuman.transform.localPosition.z);
            posToReturnTo = transform.localPosition;
            myState = EnemyState.Action;
            waitTimer = 0f;

            foreach(Capturer c in capturers)
            {
                //Parse state over to other capturers.
                c.transform.LookAt(new Vector3(c.myHuman.transform.position.x, c.transform.position.y, c.myHuman.transform.localPosition.z));
                c.aboveHuman = aboveHuman;
                c.myState = EnemyState.Action;
                c.posToReturnTo = posToReturnTo;
                c.waitTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Set the selected human.
    /// </summary>
    public void SetTarget()
    {
        //Use an index ref to avoid having to repeat this.
        int closestIndex = 0;
        //Random timer to trigger Action
        maxTime = Random.Range(2.5f, 12.5f);
        //Get closest human - only applies on first tile to make more efficient.
        Human[] humansRef = transform.parent.parent.GetChild(1).GetComponentsInChildren<Human>();
        float closestDist = 1000f;
        for(int i = 0; i < humansRef.Length; i++)
        {
            //Get an array of tile's Humans
            float distCalc = Vector3.Distance(transform.localPosition, humansRef[i].transform.localPosition);
            if (distCalc < closestDist && !humansRef[i].targeted)
            {
                //If the distance between Human h and the Capturer is lower than anything before, set the chosen Human to h
                myHuman = humansRef[i];
                closestDist = distCalc;
                closestIndex = i;
            }
        }

        //Set myHuman's targeted to prevent other captureres targeting the same human
        myHuman.targeted = true;

        //Get dupes and copy over vars
        foreach (Enemy e in enemies) capturers.Add(e.GetComponent<Capturer>());
        foreach (Capturer c in capturers)
        {
            //Set persistent variables
            c.maxTime = maxTime;
            //Copy over target by index number.
            c.myHuman = c.transform.parent.parent.GetChild(1).GetChild(closestIndex).GetComponent<Human>();
            c.myHuman.targeted = true;
        }
    }


    /// <summary>
    /// Perform action unique to this enemy. In this case, Move towards the human to capture them.
    /// </summary>
    public override void Action()
    {
        //Check if there is a human to reference
        if(myHuman != null)
        {
            //If there is, check if it's still alive and can be picked up
            if (!myHuman.gameObject.activeSelf || (myHuman.captured && (myHuman.capToFollow != this && myHuman.capToFollow != null)))
            {
                //If it's unreachable, set myHuman to null and quit out the function.
                myHuman = null;
                return;
            }
            if (!canPickUp)
            {
                //Move towards the human
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, aboveHuman, (speed * 25) * Time.deltaTime);
                if (transform.localPosition == aboveHuman)
                {
                    //If above the human, capture it
                    transform.localPosition = aboveHuman;
                    canPickUp = true;
                    myHuman.Capture();

                    beam.Play();

                    //Start playing capture loop
                    otherSound.loop = true;
                    otherSound.PlayOneShot(fxClips[0]);
                }
            }
            else
            {
                //Attach human, wait x seconds for them to appear below this
                //Vector3 belowCapturer = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
                myHuman.transform.position = Vector3.MoveTowards(myHuman.transform.position, capturePos.position, (speed * 10) * Time.deltaTime);
                if (Vector3.Distance(myHuman.transform.position, capturePos.position) < 0.1f)
                {
                    //Set to return, reset bools
                    Debug.Log("SUCCESSFULLY CAPTURED");
                    myHuman.transform.position = capturePos.position;
                    myHuman.capToFollow = this;
                    finishedJob = true;
                    beam.Stop();
                    canPickUp = false;
                    myState = EnemyState.Return;
                    //Stop loop and play new sound
                    otherSound.Stop();
                    otherSound.loop = false;
                    otherSound.PlayOneShot(fxClips[1]);

                    foreach (Capturer c in capturers)
                    {
                        //Vector3 cbelowCapturer = new Vector3(c.transform.position.x, c.transform.position.y - 1f, c.transform.position.z);

                        //Sync other capturers so they keep up.
                        c.transform.localPosition = transform.localPosition;
                        c.myHuman.transform.localPosition = c.capturePos.position;
                        c.myHuman.capToFollow = c;
                        c.finishedJob = true;
                        beam.Stop();
                        c.canPickUp = false;
                        c.myState = EnemyState.Return;
                    }
                }
            }
            
        }
        else
        {
            //If there's no human to chase, return to action.
            finishedJob = true;
            myState = EnemyState.Return;
            canPickUp = false;
            foreach(Capturer c in capturers)
            {
                //Sync other capturers so they keep up.
                c.finishedJob = true;
                c.myState = EnemyState.Return;
                c.canPickUp = false;
            }
        }
    }

    /// <summary>
    /// Call when destroyed. Similar to Enemy but affects any attached humans. 
    /// </summary>
    private void OnDestroy()
    {
        //Stop engine audio
        engineSound.Stop();
        beam.Stop();

        if (myHuman != null)
        {
            //If an attached human exists, remove any possible attachment to this object
            myHuman.UnCapture();
            myHuman.capToFollow = null;
        }

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
}
