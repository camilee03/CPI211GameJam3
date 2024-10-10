using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public int numObjectives;
    int currentNumObjectives;

    public void AccomplishedObjective()
    {
        currentNumObjectives++;
    }

    public void CheckWin(bool isDead)
    {
        if (currentNumObjectives < numObjectives || isDead)
        {
            // you loose boo restart
        }
        else
        {
            // you win yay!
        }
    }
}
