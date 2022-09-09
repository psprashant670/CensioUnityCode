using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelIntroPanel : MonoBehaviour
{
    public TextMeshProUGUI Message_txt;
    string[] Messages;
    int index = 0;
    
    void OnEnable()
    {
        index = 0;
        Messages = Data.instance.CurrentLevel.challengeIntroMessage.Split('&');
        Message_txt.SetText(Messages[index]);
    }
    

    public void NextClick()
    {
        index++;
        if (index >= Messages.Length)
        {
            gameObject.SetActive(false);
            MenuUI.instance.PlayLevel();
        }
        else
        {
            Message_txt.SetText(Messages[index]);
        }
    }
}
