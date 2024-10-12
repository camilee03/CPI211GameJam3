using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RotateText : MonoBehaviour
{
    bool change;
    public string[] texts;
    int index = 0;
    TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        StartCoroutine(Timer());
    }

    void Update()
    {
        if (change) { ChangeText(); StartCoroutine(Timer()); }
    }

    IEnumerator Timer()
    {
        change = false;
        yield return new WaitForSeconds(5);
        change = true;
    }

    private void ChangeText() {
        text.text = texts[index];
        index++;
        if (index >= texts.Length) { index = 0; }
    }
}
