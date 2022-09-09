using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadIntroMsg : MonoBehaviour
{
    public TextMeshProUGUI Intro1, Intro2;
    public TextMeshProUGUI SingleIntro;

    void Start()
    {
        if (SingleIntro != null)
        {
            if (Data.instance.CurrentLevel.attemptNumber!=1)
            {
                SingleIntro.SetText(Data.instance.GetGameAttemptDataValue(Data.instance.CurrentLevel.idLevel, Data.instance.CurrentLevel.attemptNumber).failAttemptMessage);
                return;
            }
            SingleIntro.SetText(Data.instance.CurrentLevel.challengeIntroMessage);
            return;
        }
        
        string s = Data.instance.CurrentLevel.challengeIntroMessage;
        string[] result = s.Split('&');
        Intro1.SetText(result[0]);
        if (Data.instance.CurrentLevel.attemptNumber!=1)
        {
            Intro2.SetText(Data.instance.GetGameAttemptDataValue(Data.instance.CurrentLevel.idLevel, Data.instance.CurrentLevel.attemptNumber).failAttemptMessage);
            return;
        }
        Intro2.SetText(result[1]);
    }
}
