using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Globals : MonoBehaviour
{
    //Store global data like lives left and stuffed animals collected
    [Header("Player")]
    public int playerLives = 3;
    [SerializeField] GameObject playerObject;
    Vector3 playerOrigin = new Vector3(397.94f, 0.5733376f, 293.2923f);

    [Header("Demon")]
    [SerializeField] GameObject demonObject;
    [SerializeField] Transform[] demonOrigins;

    [Header("UI")]
    [SerializeField] GameObject scareImage;
    [SerializeField] GameObject blackImage;
    [SerializeField] Image[] hearts;

    [Header("Coroutines")]
    bool isRunning;

    // win condition: called when animals are collected
    public void WinCondition ()
    {
        SceneManager.LoadScene(2); // load win scene
    }

    //called by the demon to end the game / take a life
    public void playerCaught()
    {
        playerLives--;
           
        StartCoroutine(Scare());
    }

    // pop up the jumpscare image & play the sfx
    IEnumerator Scare()
    {
        scareImage.SetActive(true);
        demonObject.SetActive(false);
        scareImage.gameObject.GetComponent<AudioSource>().Play(); // play sound
        yield return new WaitForSeconds(2);

        scareImage.SetActive(false);
        blackImage.SetActive(true);

        if (playerLives <= 0) { Application.Quit(); } // end the game, 
        else { StartCoroutine(Fade()); } // fade to black
    }

    // shows the hearts remaining
    IEnumerator Fade()
    {
        float transition = 0;
        isRunning = true;

        // Fades the menu and text out or in
        while (transition >= 0 && transition <= 1)
        {
            transition += .01f;

            // Spawns hearts as lives left
            for (int i=0; i<playerLives+1; i++)
            {
                hearts[i].color = new Color(hearts[i].color.r, hearts[i].color.g, hearts[i].color.b, transition);
            }
            yield return new WaitForEndOfFrame();
        }

        hearts[playerLives].gameObject.SetActive(false); // removes a life

        yield return new WaitForSeconds(3);
        blackImage.SetActive(false);

        playerObject.transform.position = playerOrigin; // resets the player
        print(playerOrigin);

        demonObject.SetActive(true);
        Vector3 demonOrigin = demonOrigins[Random.Range(0, demonOrigins.Length)].position;
        demonObject.transform.position = demonOrigin; // resets the demon

        isRunning = false;

    }

}
