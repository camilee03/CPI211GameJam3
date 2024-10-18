using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    public int numObjectives;
    int currentNumObjectives;

    [SerializeField] GameObject WinScreen;
    [SerializeField] GameObject LooseScreen;

    [SerializeField] AudioSource scream;
    [SerializeField] TMP_Text text;
    private void Start()
    {
        text.text = "Current Objectives: " + currentNumObjectives + "/" + numObjectives;
    }
    public void AccomplishedObjective()
    {
        currentNumObjectives++;
        text.text = "Current Objectives: " + currentNumObjectives + "/" + numObjectives;
    }

    public void CheckWin(bool isDead)
    {
        if (currentNumObjectives < numObjectives || isDead)
        {
            // you loose boo restart
            scream.Play();
            LooseScreen.SetActive(true);
            ReloadScene();
        }
        else if (currentNumObjectives > numObjectives)
        {
            // you win yay!
            WinScreen.SetActive(true);
            ReloadScene();
        }
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }
}
