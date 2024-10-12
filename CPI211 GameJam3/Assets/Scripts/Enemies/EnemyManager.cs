using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonManager : MonoBehaviour
{
    //A AI director script that tries to manage the movement and detection of a Lethal Company style enemy

    public DemonMovement DemonMovement; //we'll update the .target when the last known position changes
    public Transform actualPlayerLocation;
    public Transform lastKnownPosition;
    [SerializeField] Globals globals;
    [SerializeField] AudioSource demonAudio;
    [SerializeField] GameObject demonPointsParent;
    private List<GameObject> demonPoints = new List<GameObject>();
    [SerializeField] GameObject hiddenPointsParent;
    private List<GameObject> hidePoints = new List<GameObject>();
    [SerializeField] int farDistance; //distance past which the demon will speed up
    [SerializeField] int speedIncrease;
    private Transform demonTransform ;
    [SerializeField] int fixedUpdatesToSpot = 50; //at 0.02 sec per update (default) this makes spot time 1s of being seen
    private int consecutiveSpotsCounter = 0;
    [SerializeField] float demonSightRange = 10f;
    [SerializeField] float demonSenseRange = 3f; //the radius in which the demon knows where you are (so you cant just circle it to confuse the AI)
    [SerializeField] float demonFOV = 45f;
    [SerializeField] float demonCatchRange = 0.5f; //range used to check if the demon caught the player or arrived at the last known
    [SerializeField] int searchDuration = 20;

    private bool lockedOn = false;
    private bool wandering = true; //wandering behavior modeled off of Lethal Company (Jester Enemy)
    //Demon will wander between demonPoints, clearing off nearby ones untill all are checked, and then reset them
    private List<GameObject> wanderPointsLeft = new List<GameObject>();
    private GameObject wanderTarget;
    void Start()
    {
        if (globals == null)
        {
            globals = GameObject.FindGameObjectWithTag("Globals").GetComponent<Globals>();
            if (globals == null)
            {
                Debug.Log("Demon Manager failed to find globals, make sure it exists and is tagged");
            }
        }

        //get the DemonMovement so we can control it in future
        demonTransform = GetChildWithTag(transform, "Demon");

        if (demonTransform != null)
        {
            DemonMovement = demonTransform.GetComponent<DemonMovement>();
            demonAudio = demonTransform.GetComponent<AudioSource>();
        }
        else
        {
            Debug.Log("Demon system couldn't find the demon object.");
        }
        //add all children of the point parents to their respective lists
        foreach (Transform demonPoint in demonPointsParent.transform)

        {
            demonPoints.Add(demonPoint.gameObject);
            wanderPointsLeft.Add(demonPoint.gameObject);
        }
        foreach (Transform hidePoint in hiddenPointsParent.transform)
        {
            hidePoints.Add(hidePoint.gameObject);
        }

        pickFirstWanderTarget();
    }
    Transform GetChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
        }
        return null;
    }

    //Checks if the player is in one of the predefined hide points
    bool checkPlayerHidden(Transform player)
    {
        if (player == null)
            return false;
        //check through all of the hide points, and for each, check if the player is within their box collider
        //if the player is in one of them, return true, else false
        foreach (GameObject hidePoint in hidePoints)
        {
            BoxCollider boxCollider = hidePoint.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                // Check if the player is within the bounds of this BoxCollider
                if (boxCollider.bounds.Contains(player.position))
                {
                    return true;  // Player is hidden
                }
            }
        }

        return false;  // Player is not hidden by the hide point system
    }

    //Use FOV based LOS detection to see if the demon can see the player
    bool canSeePlayer(Transform playerPosition, Transform demonPosition, float sightRange, float fieldOfView)
    {
        if (playerPosition == null)
            return false;

        //check if player in sight range
        Vector3 directionToPlayer = playerPosition.position - demonPosition.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > sightRange)
        {
            return false;
        }

        //check if the player is in the FOV
        float angleToPlayer = Vector3.Angle(demonPosition.forward, directionToPlayer);
        if (angleToPlayer > fieldOfView / 2)
        {
            return false;
        }

        //now check if the ghost has LOS on the playerin the sightRange
        RaycastHit hit;
        if (Physics.Raycast(demonPosition.position, directionToPlayer.normalized, out hit, sightRange))
        {
            // If the ray hits the player, they are in sight
            if (hit.transform == actualPlayerLocation)
            {
                return true;  // Player is in the line of sight
            }
        }

        return false;  // Player is not in sight
    }

    bool arrivedAt(Transform curPosition, Transform target, float arrivedRadius)
    {
        Vector3 directionToTarget = target.position - curPosition.position;
        float distanceToTarget = directionToTarget.magnitude;
        if (distanceToTarget <= arrivedRadius)
        return true;
        else
        return false;
    }

    List<GameObject> findNearbyGameObjects(Transform centralPosition, float radius, List<GameObject> demonPoints, out List<GameObject> nearbyPoints)
    {
        nearbyPoints = new List<GameObject>();

        //loop through the demonPoints to see if any of them are close enough, add those that are to the nearbyPoints list
        foreach (GameObject point in demonPoints)
        {
            float distance = Vector3.Distance(centralPosition.position, point.transform.position);
            if (distance <= radius)
            {
                nearbyPoints.Add(point);
            }
        }

        return nearbyPoints;
    }

    GameObject findNearestGameObjectOnList(Transform origin, List<GameObject> gameObjects)
    {
        GameObject closest = gameObjects[0];
        foreach (GameObject obj in gameObjects)
        {
            float distance = Vector3.Distance(origin.position, obj.transform.position);
            float curClosestDistance = Vector3.Distance(origin.position, closest.transform.position);
            if(curClosestDistance > distance)
            {
                closest = obj;
            }
        }
        return closest;
    }

    void resetWanderPointsLeft()
    {
        wanderPointsLeft.Clear();
        //put all demonPoints into the wanderPointsLeft
        foreach (Transform demonPoint in demonPointsParent.transform)
        {
            wanderPointsLeft.Add(demonPoint.gameObject);
        }
    }

    void pickFirstWanderTarget()
    {
        //wandering target will be a random demonPoint
        int randomIndex = Random.Range(0, wanderPointsLeft.Count);
        wanderTarget = wanderPointsLeft[randomIndex];
        lastKnownPosition = wanderTarget.transform;
        DemonMovement.target = lastKnownPosition;
    }

    //fixed update is used to spot / sense the player
    //we used fixed update to update the last known position so that spotting the player takes a consistent amount of time
    private void FixedUpdate()
    {
        bool canSee = canSeePlayer(actualPlayerLocation, demonTransform, demonSightRange, demonFOV);
        bool hidden = checkPlayerHidden(actualPlayerLocation);
        if(!lockedOn)
        {
            if (canSee && !hidden)
            {
                consecutiveSpotsCounter++;

                //if the player has been seen enough, update Last Known Position
                if (consecutiveSpotsCounter >= fixedUpdatesToSpot)
                {
                    lastKnownPosition.position = actualPlayerLocation.position;
                    DemonMovement.target = lastKnownPosition;
                    consecutiveSpotsCounter = 0;
                    lockedOn = true;
                    if (wandering) //reset the wander points when initially spotting a player (prevent reduntant clears)
                    {
                        resetWanderPointsLeft();
                        //play the spooky sounds
                        demonAudio.Play();
                    }
                    wandering = false;
                    Debug.Log("Found player - Updating target position");
                }    
            }
            else
            {
                consecutiveSpotsCounter = 0;

                //if not previously wandering, pick a new wanderTarget from the points left (we are resetting the wander)
                if (!wandering)
                {
                    pickFirstWanderTarget();
                    Debug.Log("Lost sight of player, begin wandering/searching");
                }
                else
                {
                    //mid-wander behavior
                    //check if we're at the wander target, if so, clear off nearby points and select a new target
                    //if we're out of wanderPointsLeft, reset it and pickFirstWanderTarget again to reset the wander

                    //check if we're at the wander target
                    if(arrivedAt(demonTransform, wanderTarget.transform, demonCatchRange))
                    {
                        //remove the found target
                        Debug.Log("Arrived at wander target: " + wanderTarget.gameObject.name);
                        wanderPointsLeft.Remove(wanderTarget);

                        //remove nearby points so we don't check points unnecessarily
                        List<GameObject> nearbyPoints;
                        findNearbyGameObjects(wanderTarget.transform, demonCatchRange, wanderPointsLeft, out nearbyPoints);
                        foreach (GameObject nearby in nearbyPoints)
                        {
                            wanderPointsLeft.Remove(nearby);
                        }

                        //reset if wanderPointsLeft is empty
                        if (wanderPointsLeft.Count == 0)
                        {
                            resetWanderPointsLeft();
                            pickFirstWanderTarget();
                        }
                        else
                        {
                            //select a new wanderTarget
                            GameObject newTarget = findNearestGameObjectOnList(wanderTarget.transform, wanderPointsLeft);
                            wanderTarget = newTarget;
                            Debug.Log("Selecting new wander target: " + wanderTarget.gameObject.name);
                            DemonMovement.target = wanderTarget.transform;
                        }
                    }
                }
                wandering = true;
            }
        }
        else
        {
            //the demon is locked on and chasing the player
            
            //is it so close that it can sense the player?
            Vector3 directionToPlayer = actualPlayerLocation.position - demonTransform.position;
            float distanceToPlayer = directionToPlayer.magnitude;
            bool inSenseRange = false;
            if (distanceToPlayer <= demonSenseRange)
            {
                inSenseRange = true;
            }

            //continually lock on to the player and keep following them
            if ((canSee || inSenseRange) && !hidden)
            {
                lastKnownPosition.position = actualPlayerLocation.position;
                if (distanceToPlayer <= demonCatchRange) {
                    globals.playerCaught();
                }
            }
            else //if the demon can't see or sense the player, it'll lose the lock and begin wandering
            {
                lockedOn = false;
            }

        }
    }
}
