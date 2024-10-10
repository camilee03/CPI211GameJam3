using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LilGuyAI : MonoBehaviour
{
    public Transform goal;
    public bool canStart;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (canStart) { agent.destination = goal.position; }
    }

    public void SoundMove(bool attracts, Vector3 target)
    {
        if (attracts)
        {
            agent.destination = target;
        }
        else
        {
            agent.destination = - target;
        }
    }
}
