using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    [SerializeField] AudioSource clip;
    [SerializeField] bool attracts; // whether the sound attracts or repels npc
    float radius = 10;
    [SerializeField] bool toggle;

    private void Start()
    {
        StartCoroutine(ToggleSound());
    }

    private void Update()
    {
        if (toggle) { MakeSound(); }
    }

    public void MakeSound()
    {
        clip.Play();

        Vector3 center = this.transform.position;
        Collider[] hits = Physics.OverlapSphere(center, radius, 0, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject.tag == "Player")
            {
                Debug.Log($"{hit.gameObject.name} was hit");
                hit.gameObject.GetComponent<LilGuyAI>().SoundMove(attracts, center);
            }
        }

        StartCoroutine(ToggleSound());
    }

    IEnumerator ToggleSound()
    {
        toggle = false;
        int sec = Random.Range(5, 15);
        yield return new WaitForSeconds(sec);
        toggle = true;
    }
}
