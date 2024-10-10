using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class SoundSystem : MonoBehaviour
{
    [SerializeField] AudioSource clip;
    [SerializeField] bool attracts; // whether the sound attracts or repels npc
    float radius = 10;
    [SerializeField] bool toggle;

    private void Update()
    {
        if (toggle) { MakeSound(); }
    }

    public void MakeSound()
    {
        //clip.Play();

        Vector3 center = this.transform.position;
        Collider[] hits = Physics.OverlapSphere(center, radius);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject.tag == "Player")
            {
                Debug.Log($"{hit.gameObject.name} was hit");
                hit.gameObject.GetComponent<LilGuyAI>().SoundMove(attracts, center);
            }
        }

        toggle = false;
    }
}
