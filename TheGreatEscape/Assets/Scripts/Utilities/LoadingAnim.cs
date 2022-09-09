using System.Collections;
using UnityEngine;
using TMPro;

public class LoadingAnim : MonoBehaviour
{
    public TextMeshProUGUI Loading_txt;

    public string[] Texts;
    int counter = 0;

    void OnEnable()
    {
        if (Texts.Length > 1)
        {
            StartCoroutine(Animate());
        }
    }

    void OnDisable() 
    {
        StopCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.69f);
            Loading_txt.SetText(Texts[counter]);
            counter++;
            if (counter >= Texts.Length)
            {
                counter = 0;
            }
        }
    }
}
