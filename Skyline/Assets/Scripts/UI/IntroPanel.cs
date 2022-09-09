using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroPanel : MonoBehaviour
{
    public TextMeshProUGUI Message_txt;

    List<string> Messages;
    int index = 0;
    
    void OnEnable()
    {
        index = 0;
        Messages = Data.instance.IntroMessages;
        Message_txt.SetText(Messages[index]);
    }
    

    public void NextClick()
    {
        index++;
        if (index >= Messages.Count)
        {
            MenuUI.instance.LevelSelectPanel.SetData();
            MenuUI.instance.LevelSelectPanel.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            Messages = Data.instance.IntroMessages;
            Message_txt.SetText(Messages[index]);
        }
    }
}
