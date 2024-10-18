using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public int radius;
    int movementIndex;
    NavMeshAgent agent;
    bool moving;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // randomize which movement enemy performs
        movementIndex = Random.Range(0, 3);
    }
    private void Update()
    {
        switch (movementIndex)
        {
            case 0: Stationary(); break;
            case 1: Movement1(); break;
            case 2: Movement2(); break;
            default: break;
        }

        DetectPlayer();

        if (agent.velocity == Vector3.zero) { moving =  false; }
    }

    private void Stationary() { }
    private void Movement1() {
        int move = Random.Range(-5, 5);
        if (!moving) { agent.destination = transform.position + Vector3.forward * move; }
    }
    private void Movement2() {
        int move = Random.Range(-2, 2);
        if (!moving) { agent.destination = transform.position + Vector3.forward * move; }
    }

    private void DetectPlayer()
    {
        Vector3 center = this.transform.position;
        Collider[] hits = Physics.OverlapSphere(center, radius, 5, QueryTriggerInteraction.Collide); // maybe change to a cone or something for sight?

        foreach (Collider hit in hits)
        {
            if (hit.gameObject.tag == "Player")
            {
                moving = true;
                Debug.Log($"{hit.gameObject.name} was found");
                // change destination to player
                agent.destination = hit.gameObject.transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponentInChildren<WinCondition>().CheckWin(true);
        }
    }
}
