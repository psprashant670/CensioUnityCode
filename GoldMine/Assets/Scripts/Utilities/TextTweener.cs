using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTweener : MonoBehaviour
{
    [Multiline]
    public string Text;
    public TextMeshProUGUI TextBox;
    public float TimeBetweenChar;

    char [] stringArr;
    WaitForSeconds waitFor;
    string result;

    void Start()
    {
        waitFor = new WaitForSeconds(TimeBetweenChar);
        if (TextBox==null)
        {
            TextBox = GetComponent<TextMeshProUGUI>();
        }

        StartTween();
    }

    public void StartTween()
    {
        stringArr = Text.ToCharArray();
        StopCoroutine("DoTextTween");
        StartCoroutine("DoTextTween");
    }

    IEnumerator DoTextTween()
    {
        result = "";
        for (int i = 0; i < stringArr.Length; i++)
        {
            result += stringArr[i];
            TextBox.SetText(result);
            yield return waitFor;
        }
    }
}
