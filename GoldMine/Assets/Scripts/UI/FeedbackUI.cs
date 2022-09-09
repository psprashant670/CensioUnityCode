using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FeedbackUI : MonoBehaviour
{
    public TextMeshProUGUI Question_txt;
    public List<FeedbackOptionUI> Options;
    public List<string> Questions;
    int index = 0;
    public string Result;

    void Start()
    {
        index = 0;
        Result = "";
        for (int i = 0; i < Data.instance.SubliminalMeasurement2Data.Count; i++)
        {
            Options[i].SubliminalMeasurement2Data = Data.instance.SubliminalMeasurement2Data[i];
            Options[i].SetData();
        }

        Question_txt.SetText(Questions[index]);
    }

    public void ShowPanel(bool _state)
    {
        gameObject.SetActive(_state);
        InGameUI.instance.HUDPanel.SetActive(false);
    }

    public void UncheckAll()
    {
        foreach (FeedbackOptionUI _FeedbackOptionUI in Options)
        {
            _FeedbackOptionUI.SetOption(false);
        }
    }

    public void OnNextClick()
    {
        string checkedID = "";
        for (int i = 0; i < Options.Count; i++)
        {
            if (Options[i].isChecked)
            {
                checkedID = Options[i].ID;
            }
        }
        if (checkedID=="")
        {
            return;
        }
        Result += checkedID;

        index++;
        if (LevelManager.instance.CurrentLevel == Level.Level2 && index == Questions.Count-1)
        {
            Options[0].Option_txt.SetText("I was lucky");
            Options[1].Option_txt.SetText("It was easy");
            Options[2].Option_txt.SetText("I am good at these challenges");
            Options[3].Option_txt.SetText("I am smart");
            Options[4].Option_txt.SetText("I made an effort to solve these challenges");
        }

        if (index >= Questions.Count)
        {
            Complete();
            return;
        }
        Question_txt.SetText(Questions[index]);
        UncheckAll();
    }

    public void Complete()
    {
        // Data.instance.CurrentLevel.idSubliminalMeasurement
        GameManager.instance.MenuState = MenuState.LevelSelect;
        Data.instance.PostUserFeedback_Co(GameManager.instance, Result);
        Debug.Log(Result);
        GameManager.instance.LoadScene("Menu");
    }
}
