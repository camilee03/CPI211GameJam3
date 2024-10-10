using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSystem : MonoBehaviour
{
    float radius = 10;
    bool toggle = true;

    private void Update()
    {
        if (toggle) { StartObjective(); }
    }

    public void StartObjective()
    {
        Vector3 center = this.transform.position;
        Collider[] hits = Physics.OverlapSphere(center, radius);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject.tag == "Player")
            {
                Debug.Log($"{hit.gameObject.name} was hit");
                hit.gameObject.GetComponent<LilGuyAI>().ObjectiveMove(center);
                hit.gameObject.GetComponentInChildren<WinCondition>().AccomplishedObjective();
            }
        }

        toggle = false;
    }
}
