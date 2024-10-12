using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static System.Math;

public class LilGuyAI : MonoBehaviour
{
    Transform goal;
    public Transform[] goals;
    int goalPos;
    public bool canStart;
    NavMeshAgent agent;
    Vector3 soundPosition;
    Vector3 objPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        goal = goals[0];
    }
    private void Update()
    {
        if (canStart) {
            agent.destination = goal.position;
            agent.stoppingDistance = 1;

            // Debug.Log(transform.position + ":" + goal.position);
            // soundPosition = goal.position; // reset sound position
            // objPosition = goal.position; // reset objective position
        }
        else {
            canStart = agent.isStopped;
            
            // possibly have some UI indicating completion??
        }
    }

    public void SoundMove(bool attracts, Vector3 target)
    {
        canStart = false;
        if (attracts)
        {
            agent.destination = target;
            soundPosition = target;
        }
        else
        {
            agent.destination = - target;
            soundPosition = -target;
        }
    }

    public void ObjectiveMove(Vector3 target)
    {
        canStart = false;
        agent.destination = target;
        objPosition = target;
    }

    private void ChangeGoal()
    {
        goalPos++;
        if (goalPos < goals.Length) { goal = goals[goalPos]; }
        else { GetComponentInChildren<WinCondition>().CheckWin(false); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Destination")
        {
            print("AHHHH");
            ChangeGoal();
        }
    }
}
