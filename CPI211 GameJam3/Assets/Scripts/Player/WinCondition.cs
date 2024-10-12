using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    public int numObjectives;
    int currentNumObjectives;

    [SerializeField] GameObject WinScreen;
    [SerializeField] GameObject LooseScreen;

    [SerializeField] AudioSource scream;

    public void AccomplishedObjective()
    {
        currentNumObjectives++;
    }

    public void CheckWin(bool isDead)
    {
        if (currentNumObjectives < numObjectives || isDead)
        {
            // you loose boo restart
            scream.Play();
            LooseScreen.SetActive(true);
        }
        else
        {
            // you win yay!
            WinScreen.SetActive(true);
        }
        ReloadScene();
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
